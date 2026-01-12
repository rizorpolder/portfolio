using Game.Scripts.Systems.Input.Data;
using UnityEngine;

namespace Game.Scripts.Systems.Input
{
	public class MouseZoomInputHandler : ZoomInputHandler
	{
		public override void Handle()
		{
			if (!Filter())
				return;
            
			if (!UnityEngine.Input.GetKey(KeyCode.Mouse2) && UnityEngine.Input.mouseScrollDelta.y.Equals(0))
				return;

			var inputParametersValue = _inputHandler.InputParameters;
			var delta = UnityEngine.Input.mouseScrollDelta.y * inputParametersValue.MouseZoomSensitivity;
			if (delta == 0)
				return;

			var newOrtho = CalculateNextOrtho(delta, inputParametersValue);
			_inputHandler.SetZoomTarget(new CameraZoomTarget(newOrtho, 0f, ResetZoom, true));
			_inputHandler.Zooming = true;
		}
	}
}