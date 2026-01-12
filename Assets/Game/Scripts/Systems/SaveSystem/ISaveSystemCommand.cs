using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Game.Scripts.Systems.SaveSystem
{
	public interface ISaveSystemCommand
	{
		public UniTask SaveForce(Action onComplete = null);

		public UniTask SaveForce(SaveDataType newValue, Action onComplete = null);

		public UniTask Save(SaveDataType saveRequest, bool force = false, Action callback = null);

		public UniTask<(bool success, T data)> LoadData<T>(string fileName);

		public UniTask<string> LoadData(string fileName);

		public UniTask RemoveData(string key);

		public UniTask SyncData();



		// public void Save(SaveDataType saveRequest, bool force = false);
		// T LoadData<T>(SaveDataType dataType);
	}
}