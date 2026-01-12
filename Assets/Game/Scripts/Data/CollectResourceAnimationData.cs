using System;
using UI.HUD.ResourceCollector;

namespace Game.Scripts.Data
{
	public class CollectResourceAnimationData
	{
		public Resource resource;
		public ResourceCollectTarget start;
		public Action onComplete;
		public bool isChangeSize;
		public ResourceCollectTarget? finish;

		public CollectResourceAnimationData(Resource resource,
			ResourceCollectTarget start,
			System.Action callback,
			bool changeSize = false)
		{
			this.resource = resource;
			this.start = start;
			onComplete = callback;
			isChangeSize = changeSize;
		}

		public void SetFinish(ResourceCollectTarget finish)
		{
			this.finish = finish;
		}
	}
}