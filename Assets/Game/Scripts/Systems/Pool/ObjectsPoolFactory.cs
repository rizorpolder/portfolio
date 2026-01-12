using UnityEngine;
using Zenject;

namespace Game.Scripts.Common.Pool
{
	public class ObjectsPoolFactory
	{
		private DiContainer _diContainer;

		[Inject]
		public ObjectsPoolFactory(DiContainer diContainer)
		{
			_diContainer = diContainer;
		}

		public ObjectsPool<T> CreatePool<T>(T prefab, Transform parent) where T : MonoBehaviour
		{
			var result = new ObjectsPool<T>(parent, prefab);
			_diContainer.Inject(result);
			return result;
		}
	}
}