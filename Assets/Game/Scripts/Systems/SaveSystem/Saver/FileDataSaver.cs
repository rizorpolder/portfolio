using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Systems.SaveSystem.Saver
{
	public class FileDataSaver : ADataSaver
	{
		private static readonly string mainSavePath = Application.persistentDataPath;

		public override bool IsSupported(StorageType storageType)
		{
			return storageType == StorageType.LocalStorage;
		}

		public override async UniTask Save(string key, string value, StorageType? storageType = null)
		{
			var byteArr = Encoding.UTF8.GetBytes(value);
			File.WriteAllBytes($"{mainSavePath}/{key}", byteArr);
			await UniTask.CompletedTask;
		}

		public override async UniTask<(bool success, string result)> TryLoad(string key,
			StorageType? storageType = null)
		{
			string result = string.Empty;

			var path = $"{mainSavePath}/{key}";
			if (!File.Exists(path)) return (false, result);

			try
			{
				var data = File.ReadAllBytes(path);
				result = Encoding.UTF8.GetString(data);
			}
			catch (Exception e)
			{
				Debug.LogAssertionFormat(e.Message);
				return (false, result);
			}

			return (true, result);
		}

		public override async UniTask<Dictionary<string, string>> Load(string[] keys,
			StorageType storageType = StorageType.LocalStorage)
		{
			Dictionary<string, string> results = new Dictionary<string, string>();

			foreach (var key in keys)
			{
				var loadResult = await TryLoad(key, storageType);

				if (loadResult.success)
				{
					results.Add(key, loadResult.result);
				}
			}

			return results;
		}

		public override async UniTask TryRemoveData(string key, StorageType storageType = StorageType.LocalStorage)
		{
			if (File.Exists($"{mainSavePath}/{key}"))
			{
				File.Delete($"{mainSavePath}/{key}");
			}
		}
	}
}