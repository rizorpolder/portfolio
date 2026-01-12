using Zenject;

namespace Game.Scripts.Systems.Initialize
{
	public class SceneContainerCoordinator
	{
		private DiContainer _container;

		public void SetupContainer(DiContainer container) => _container = container;
		public DiContainer GetContainer() => _container;
	}
}