using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AudioManager.Runtime.Extensions;
using Cysharp.Threading.Tasks;
using Game.Scripts.Data.SaveData;
using Game.Scripts.Extensions;
using Game.Scripts.Systems.Initialize;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.Network.ApiData.PlayerData;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.QuestSystem;
using Game.Scripts.Systems.SaveSystem.Operations;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.SaveSystem
{
	[Flags]
	public enum SaveDataType
	{
		None = 1 << 0,
		PlayerData = 1 << 1,
		QuestData = 1 << 2,
		Session = 1 << 4,

		Statistics = 1 << 5,
		MetaData = 1 << 7,
		All = PlayerData | QuestData | Statistics | Session | MetaData
	}

	public class SaveDataService : ISaveSystemCommand, IDisposable
	{
		private readonly float _saveDelay = 10;

		private CancellationTokenSource _cts;
		private bool _fileSystemSyncInProgress;

		private bool _isForce;

		[Inject] private NetworkConfig _networkConfig;
		[Inject] private NetworkSystem _networkSystem;
		[Inject] private IPlayerCommand _playerCommand;
		[Inject] private IPlayerData _playerData;

		[Inject] private IQuestCommand _questCommand;
		[Inject] private RequestsFactory _requestsFactory;

		[Inject] private SaveOperationFactory _saveOperationFactory;
		[Inject] private ProjectDataLoader _projectDataLoader;

		private readonly IDataSaver _saver;
		private SaveDataType _saveRequests;
		private readonly IDataSerializer _serializer;

		public SaveDataService(IDataSaver saver, IDataSerializer serializer)
		{
			_saver = saver;
			_serializer = serializer;
			StartDelayedSaving();
		}

		public void Dispose()
		{
		}

		public async UniTask SaveForce(Action onComplete = null)
		{
			await Save(SaveDataType.All, true, onComplete);
		}

		public async UniTask SaveForce(SaveDataType newValue, Action onComplete = null)
		{
			await Save(newValue, true, onComplete);
		}

		public async UniTask Save(SaveDataType saveRequest, bool force = false, Action callback = null)
		{
			if (force)
				Debug.Log($"$Force saving: {string.Join('|', saveRequest)}");

			if (!_saveRequests.HasFlag(saveRequest)) _saveRequests |= saveRequest;

			_isForce |= force;

			if (_isForce)
			{
				await SaveData();
				callback?.Invoke();
			}
		}

		public async UniTask<(bool success, T data)> LoadData<T>(string fileName)
		{
			T data = default;
			var loadResult = await _saver.TryLoad(fileName);

			if (loadResult.success)
			{
				data = _serializer.Deserialize<T>(loadResult.result);
				return (true, data);
			}

			return (false, data);
		}

		public async UniTask<string> LoadData(string fileName)
		{
			var loadResult = await _saver.TryLoad(fileName);

			if (loadResult.success) return loadResult.result;

			return string.Empty;
		}

		public async UniTask RemoveData(string key)
		{
			await _saver.TryRemoveData(key);
		}

		private void StartDelayedSaving()
		{
			if (_cts != null)
			{
				_cts.Cancel();
				_cts.Dispose();
			}

			_cts = new CancellationTokenSource();
			SaveDelayedAsync(_saveDelay, _cts.Token);
		}

		private async UniTask SaveDelayedAsync(float delay, CancellationToken cancellationToken)
		{
			await UniTask.WaitForSeconds(delay, cancellationToken: cancellationToken);

			if (!cancellationToken.IsCancellationRequested)
				await SaveData();
		}

		private async UniTask SaveData()
		{
			_isForce = false;

			if (_fileSystemSyncInProgress)
				return;

			var timestamp = DateTimeOffset.Now;

			await SaveData(_saveRequests.GetFlags(), timestamp);

			_saveRequests = SaveDataType.None;
			StartDelayedSaving();
		}

		private async UniTask SaveData(IEnumerable<Enum> saveFlags, DateTimeOffset timestamp)
		{
			var _saveOperations = new List<ASaveOperation>();
			foreach (SaveDataType type in saveFlags)
			{
				if (type == SaveDataType.None || type == SaveDataType.All)
					continue;

				var saveOperation = _saveOperationFactory.CreateSaveOperation(type);
				saveOperation.PrepareData();

				_saveOperations.Add(saveOperation);
			}

			var savesDictionary = _saveOperations.ToDictionary(op => op.SaveKey, op => op.Data);

			await _saver.Save(savesDictionary);
			if (_networkConfig.SyncWithServer)
				await SaveToServer();


			// _fileSystemSyncInProgress = true;
			//
			// FileSystem.Syncfs((err) =>
			// {
			// 	if (!string.IsNullOrEmpty(err))
			// 		Debug.LogError($"Sync FileSystem failed, data may not be written to disk!\n{err}");
			//
			// 	float delta = Time.realtimeSinceStartup - start;
			//
			// 	Debug.Log($"saved to disk:{string.Join('|', saveFlags)}\nfor {delta} seconds");
			//
			// 	_fileSystemSyncInProgress = false;
			//
			// 	if (_saveRequests != DataTypes.None)
			// 	{
			// 		Debug.Log($"Continue saving after syncfs: {{{string.Join('|', _saveRequests)}}}");
			// 		Save(_saveRequests, true).Forget();
			// 	}
			// });
		}

		public async UniTask SyncData()
		{
			var serverDataStr = await _networkSystem.LoadPlayerData();

			if (serverDataStr.IsNullOrEmpty())
			{
				await ApplyLocalData();
				return;
			}

			var serverData = _serializer.Deserialize<Dictionary<string, string>>(serverDataStr);

			var localMetaDataTask = await LoadData<SaveMetaData>(nameof(SaveDataType.MetaData));

			if (localMetaDataTask.success)
			{
				if (serverData.TryGetValue(nameof(SaveDataType.MetaData), out var serverMetaDataStr))
				{
					var serverMetaData = _serializer.Deserialize<SaveMetaData>(serverMetaDataStr);
					if (localMetaDataTask.data.UpdateDate > serverMetaData.UpdateDate)
					{
						await ApplyLocalData();
						return;
					}

					await ApplyServerData(serverData);
				}
				else
				{
					//нет на сервере мета даты
					await ApplyLocalData(); // локальные данные на сервер
				}
			}
			else
			{
				await ApplyServerData(serverData);
			}
		}

		private async UniTask SaveToServer()
		{
			if (!_networkSystem.IsAuthorized)
				return;

			var allFlags = SaveDataType.All.GetFlags();
			var saveOperations = new List<ASaveOperation>();

			foreach (SaveDataType type in allFlags)
			{
				if (type is SaveDataType.None or SaveDataType.All or SaveDataType.MetaData)
					continue;

				var saveOperation = _saveOperationFactory.CreateSaveOperation(type);
				saveOperation.PrepareData();
				saveOperations.Add(saveOperation);
			}

			var savesDictionary = saveOperations.ToDictionary(op => op.SaveKey, op => op.Data);
			var serializedMeta = _serializer.Serialize(new SaveMetaData {UpdateDate = DateTimeOffset.Now});
			savesDictionary.Add(nameof(SaveDataType.MetaData), serializedMeta);


			var data = _serializer.Serialize(savesDictionary);
			var reqData = new SavePlayerRequestData
			{
				playerData = data
			};

			await _networkSystem.SavePlayerData(reqData);
			await _saver.Save(nameof(SaveDataType.MetaData), serializedMeta);
		}

		private async UniTask ApplyLocalData()
		{
			Debug.Log($"[Saver] Apply local data");

			var allFlags = SaveDataType.All.GetFlags();
			var saveOperations = new List<ASaveOperation>();

			foreach (SaveDataType type in allFlags)
			{
				if (type is SaveDataType.None or SaveDataType.All or SaveDataType.MetaData)
					continue;

				var saveOperation = _saveOperationFactory.CreateSaveOperation(type);
				saveOperations.Add(saveOperation);
			}

			var saveKeys = saveOperations.Select(x => x.SaveKey).ToList();
			saveKeys.Add(nameof(SaveDataType.MetaData));

			var saveKeysArr = saveKeys.ToArray();
			var syncedLoadResult = await _saver.Load(saveKeysArr);
			if (syncedLoadResult.Count > 0)
			{
				var data = _serializer.Serialize(syncedLoadResult);
				var reqData = new SavePlayerRequestData
				{
					playerData = data
				};

				await _networkSystem.SavePlayerData(reqData);
			}
		}

		private async UniTask ApplyServerData(Dictionary<string, string> data)
		{
			Debug.Log($"[Saver] Apply server data");

			SaveMetaData saveMetaData = null;
			if (data.TryGetValue(nameof(SaveDataType.MetaData), out var metaDataStr))
				saveMetaData = _serializer.Deserialize<SaveMetaData>(metaDataStr);
			else
				saveMetaData = new SaveMetaData();

			saveMetaData.UpdateDate = DateTimeOffset.Now;
			data.TryAdd(nameof(SaveDataType.MetaData), _serializer.Serialize(saveMetaData));

			await _saver.TryRemoveData(data.Keys.ToArray());


			await _saver.Save(data);
			await foreach (var _ in _projectDataLoader.UpdateLoadingData()) ;
		}
	}
}