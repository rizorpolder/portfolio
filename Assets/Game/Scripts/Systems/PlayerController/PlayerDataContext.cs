using System;
using System.Linq;
using Game.Scripts.Data;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.PlayerController.Data;
using Game.Scripts.Systems.SaveSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.PlayerController
{
	public class PlayerDataContext : IPlayerData, IPlayerListener, IPlayerCommand, IInitializable
	{
		public event Action<int, int> OnPlayerPointChanged;
		public event Action OnPlayerDataUpdated;
		private PlayerData _playerData;

		public bool IsGameFinished => _playerData.IsGameFinished;

		public int PlayerSelectedIndexView => _playerData.SelectedIndex;
		public FloorNumber PlayerFloorNumber => _playerData.CurrentFloor;
		public string PlayerID => _playerData.ID;

		[Inject] private ISaveSystemCommand _saveSystemCommand;
		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<PlayerData> _playerDataCache;

		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
		}

		private void OnCacheReadyHandler()
		{
			_playerData = _playerDataCache.GetData();
			OnPlayerDataUpdated?.Invoke();
		}

		private void AddPoints(int points)
		{
			if (points < 0) return;

			int previous = _playerData.Points;

			_playerData.Points += points;
			OnPlayerPointChanged?.Invoke(previous, _playerData.Points);
		}

		public void SetPlayerInfo(string name, string surname)
		{
			_playerData.Name = name;
			_playerData.ID = $"{name}_{surname}";
		}
		
		public void SetPlayerSelectedIndex(int selectedIndex)
		{
			_playerData.SelectedIndex = selectedIndex;
		}

		public void SetPlayerGameComplete()
		{
			_playerData.IsGameFinished = true;
			_saveSystemCommand.Save(SaveDataType.PlayerData);
		}

		public void SetTutorialCompleted(string currentStep)
		{
			_playerData.LastTutorialID = currentStep;
			_saveSystemCommand.Save(SaveDataType.PlayerData);
		}

		public int GetResourceCount(Resource resource)
		{
			switch (resource.Type)
			{
				case ResourceType.Free:
					break;
				case ResourceType.Points:
					return _playerData.Points;
			}

			return 0;
		}

		public void SavePlayerPosition(string characterName, Vector3 position)
		{
			var data = _playerData.Positions.FirstOrDefault(x => x.CharacterID.Equals(characterName));
			if (data == null)
				_playerData.Positions.Add(new CharacterPositionData(characterName, position));
			else
			{
				data.CharacterID = characterName;
				data.Position = position;
			}

			_saveSystemCommand.Save(SaveDataType.PlayerData);
		}

		public bool HavePlayerPositions(string characterID, out Vector3 position)
		{
			position = Vector3.zero;
			var data = _playerData.Positions.FirstOrDefault(x => x.CharacterID.Equals(characterID));
			if (data == null)
				return false;

			position = data.Position;
			return true;
		}

		public void ChangePlayerFloor(FloorNumber floorNumber)
		{
			_playerData.CurrentFloor = floorNumber;
		}

		public PlayerData GetPlayerData()
		{
			return _playerData;
		}

		public void AddResource(Resource reward)
		{
			switch (reward.Type)
			{
				case ResourceType.Free:
					break;
				case ResourceType.Points:
					AddPoints(reward.Value);
					break;
			}
		}
	}
}