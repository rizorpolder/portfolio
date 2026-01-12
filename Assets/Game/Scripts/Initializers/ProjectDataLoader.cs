using System.Collections.Generic;
using Game.Scripts.Initializers.Interfaces;
using Game.Scripts.Initializers.Signals;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Initializers
{
	public class ProjectDataLoader
	{
		[Inject] private SignalBus _signalBus;
		private readonly IDataLoader[] _loaders;

		[Inject]
		public ProjectDataLoader(IDataLoader[] loaders)
		{
			_loaders = loaders;
		}

		public async IAsyncEnumerable<float> UpdateLoadingData()
		{
			var progressStep = 0.3f / _loaders.Length;
			for (var index = 0; index < _loaders.Length; index++)
			{
				var loader = _loaders[index];
				await loader.LoadAsync();
				var lastProgress = index * progressStep + progressStep;
				yield return lastProgress;
			}

			Debug.Log("ProjectContext: Все данные загружены.");
			_signalBus.Fire(new DataLoadedSignal());
		}
	}
}