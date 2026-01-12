using System.Collections.Generic;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.SaveSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Initialize
{
	public class ProjectDataLoader
	{
		[Inject] private SignalBus _signalBus;
		[Inject] private ISaveSystemCommand _saveSystemCommand;
		private readonly IDataLoader[] _loaders;

		[Inject]
		public ProjectDataLoader(IDataLoader[] loaders)
		{
			_loaders = loaders;
		}

		public async IAsyncEnumerable<float> UpdateLoadingData()
		{
			//await _saveSystemCommand.SyncData();

			var progressStep = 0.3f / _loaders.Length;
			for (var index = 0; index < _loaders.Length; index++)
			{
				var loader = _loaders[index];
				await loader.LoadAsync();
				var lastProgress = index * progressStep + progressStep;
				yield return lastProgress;
			}

			Debug.Log("ProjectContext: Все данные загружены.");
			_signalBus.Fire(new CacheUpdatedSignal());
		}
	}
}