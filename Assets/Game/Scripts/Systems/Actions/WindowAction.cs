using System;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.UI.WindowsSystem;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	public class WindowAction : BaseAction<WindowActionData>
	{
		public override event Action<IActionExecutable> OnActionExecuted;

		[Inject] private WindowsController _windowsController;

		public override void Execute()
		{
			_windowsController.Show(_data.WindowName,
				window =>
				{
					OnActionExecuted?.Invoke(this);
				});
		}
	}
}