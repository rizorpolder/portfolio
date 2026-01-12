using System;

namespace Game.Scripts.Systems.Actions.Interfaces
{
	public interface IActionExecutable
	{
		public event Action<IActionExecutable> OnActionExecuted;

		void Initialize(IActionData data);
		void Execute();
	}
}