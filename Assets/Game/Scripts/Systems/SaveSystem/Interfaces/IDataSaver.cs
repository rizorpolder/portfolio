using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Systems.SaveSystem
{
	public interface IDataSaver
	{
		public abstract bool IsSupported(StorageType storageType);

		public UniTask Save(string key, string value, StorageType? storageType = null);
		public UniTask Save(Dictionary<string, string> elements, StorageType? storageType = null);
		public UniTask<(bool success, string result)> TryLoad(string key, StorageType? storageType = null);

		public UniTask<Dictionary<string, string>> Load(string[] keys,
			StorageType storageType = StorageType.LocalStorage);

		public UniTask TryRemoveData(string key, StorageType storageType = StorageType.LocalStorage);
		public UniTask TryRemoveData(string[] keys, StorageType storageType = StorageType.LocalStorage);
	}
}