using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Game.Scripts.Systems.Loading.Scene
{
	public class SceneInstanceLogic
	{
		public SceneInstance Instance { get; private set; }

		public SceneInstanceLogic(SceneInstance instance)
		{
			Instance = instance;
		}

		public virtual async UniTask ActivateAsync()
		{
			await Instance.ActivateAsync().ToUniTask();
		}

		public virtual void Destroy()
		{
		}
	}
}