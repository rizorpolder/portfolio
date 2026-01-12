using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Systems.SaveSystem
{
	public abstract class ADataSaver : IDataSaver
	{
		public abstract UniTask Save(string key, string value, StorageType? storageType = null);

		public virtual async UniTask Save(Dictionary<string, string> elements, StorageType? storageType = null)
		{
			foreach (var kvp in elements)
			{
				await Save(kvp.Key, kvp.Value, storageType);
			}
		}

		public abstract UniTask<(bool success, string result)> TryLoad(string key, StorageType? storageType = null);
		public abstract UniTask TryRemoveData(string key, StorageType storageType = StorageType.LocalStorage);

		public abstract bool IsSupported(StorageType storageType);

		public virtual async UniTask<Dictionary<string, string>> Load(string[] keys,
			StorageType storageType = StorageType.LocalStorage)
		{
			var result = new Dictionary<string, string>();

			foreach (var key in keys)
			{
				var loadResult = await TryLoad(key, storageType);

				if (loadResult.success)
					result[key] = loadResult.result;
			}

			return result;
		}

		public virtual async UniTask TryRemoveData(string[] keys, StorageType storageType = StorageType.LocalStorage)
		{
			foreach (var key in keys)
				await TryRemoveData(key, storageType);
		}
	}
}