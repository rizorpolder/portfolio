using Game.Scripts.Systems.Input.Config;
using Game.Scripts.Systems.Loading;
using Game.Scripts.UI.WindowsSystem;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class ZoomInputHandler
	{
		private float _startOrtho;
		private float _delta;
		private bool _isNegativeDirection;
		
		[Inject] protected InputHandler _inputHandler;
		[Inject] private WindowsController _windowsController;

		public virtual void Initialize()
		{
		}
		
		protected bool Filter()
		{
			return !_windowsController.HasActiveWindows;
		}

		public virtual void Handle()
		{
		}

		protected virtual float CalculateNextOrtho(float delta, InputParameters parameters)
		{
			var negative = delta < 0f;
			if (_isNegativeDirection != negative)
			{
				ResetZoom();
			}

			_isNegativeDirection = negative;

			var current = _inputHandler.CameraZoom;
			if (_startOrtho == 0)
				_startOrtho = current;

			_delta += delta;
			var result = _startOrtho - _delta;
			if (result >= parameters.maxOrtho || result <= parameters.minOrtho)
				ResetZoom();

			return result;
		}

		protected void ResetZoom()
		{
			_startOrtho = 0f;
			_delta = 0f;
		}
	}
}