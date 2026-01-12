using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Initialize;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;

namespace Game.Scripts.Systems.Loading.Scene
{
	public abstract class SetupSceneLogic : SceneInstanceLogic
	{
		[Inject] private SetupCoordinator _coordinator;

		protected SetupSceneLogic(SceneInstance instance) : base(instance)
		{
		}

		public override async UniTask ActivateAsync()
		{
			await base.ActivateAsync();
			await _coordinator.GetCurrentSetup().Setup();
			
		}
	}
}