using System;
using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.UI.Common;
using Zenject;

namespace UI.HUD.ResourceCollector
{
	public class ResourceCollectService
	{
		private readonly List<CollectResourceAnimationData> _animationsInProgress =
			new List<CollectResourceAnimationData>();

		[Inject] private GlobalUI _globalUI;
		[Inject] private IPlayerCommand _playerCommand;

		public void AddAnimatedResource(Resource resource,
			ResourceCollectTarget start,
			Action onComplete = null,
			bool changeSize = false)
		{
			Collect(resource, start, onComplete, changeSize);
		}

		private void Collect(Resource resource,
			ResourceCollectTarget start,
			System.Action onComplete,
			bool changeSize = false)
		{
			var data = new CollectResourceAnimationData(resource, start, onComplete, changeSize);
			_animationsInProgress.Add(data);
			if (resource.Value < 0)
			{
				SpendResource(data);
			}
			else
			{
				CollectResource(data);
			}
		}

		private async void SpendResource(CollectResourceAnimationData data)
		{
			var collectorBehaviour = _globalUI.HUD.ResourceCollectorBehaviour;
			await collectorBehaviour.SpendResource(data);
			OnComplete(data);
		}

		private async void CollectResource(CollectResourceAnimationData data)
		{
			var collectorBehaviour = _globalUI.HUD.ResourceCollectorBehaviour;
			await collectorBehaviour.CollectResource(data);
			OnComplete(data);
		}

		private void OnComplete(CollectResourceAnimationData data)
		{
			_playerCommand.AddResource(data.resource);

			_animationsInProgress.Remove(data);
			data.onComplete?.Invoke();
		}

		public bool HasAnimatedResource()
		{
			return _animationsInProgress.Count > 0;
		}
	}
}