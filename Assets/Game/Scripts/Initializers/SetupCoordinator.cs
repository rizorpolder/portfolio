using Game.Scripts.Systems.Initialize.Interfaces;
using Game.Scripts.Systems.Initialize.Signals;

namespace Game.Scripts.Systems.Initialize
{
	public class SetupCoordinator
	{
		private ISetup _currentSetup;

		public void SetSetup(SceneDataLoadedSignal signal)
		{
			_currentSetup = signal.SetupInstance;
		}

		public ISetup GetCurrentSetup() => _currentSetup;
	}
}