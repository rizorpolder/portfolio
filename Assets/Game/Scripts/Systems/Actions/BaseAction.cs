using System;
using Game.Scripts.Systems.Actions.Interfaces;

namespace Game.Scripts.Systems.Actions
{
	public abstract class BaseAction<T> : IActionExecutable where T : IActionData
	{
		public abstract event Action<IActionExecutable> OnActionExecuted;

		protected T _data;


		public void Initialize(IActionData data)
		{
			_data = (T) data;
		}

		public abstract void Execute();
	}
}