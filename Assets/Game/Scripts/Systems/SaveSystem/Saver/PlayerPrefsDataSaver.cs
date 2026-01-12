using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Systems.SaveSystem.Saver
{
	public class PlayerPrefsDataSaver : ADataSaver
	{
		public override bool IsSupported(StorageType storageType)
		{
			return storageType == StorageType.LocalStorage;
		}

		public override async UniTask Save(string key, string value, StorageType? storageType = null)
		{
			PlayerPrefs.SetString(key, value);
			PlayerPrefs.Save();
		}

		public override async UniTask<(bool success, string result)> TryLoad(string key,
			StorageType? storageType = null)
		{
			string result = string.Empty;
			string fileName = key + ".json";

			if (PlayerPrefs.HasKey(key))
			{
				result = PlayerPrefs.GetString(key, null);
				return (result != null, result);
			}

			return (false, result);
		}

		public override async UniTask TryRemoveData(string key, StorageType storageType = StorageType.LocalStorage)
		{
			if (PlayerPrefs.HasKey(key))
				PlayerPrefs.DeleteKey(key);
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
	}
}