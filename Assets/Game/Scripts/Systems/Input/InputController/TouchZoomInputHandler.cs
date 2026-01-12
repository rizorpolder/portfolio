using System;
using Game.Scripts.Systems.Input.Config;
using Game.Scripts.Systems.Input.Data;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class TouchZoomInputHandler : ZoomInputHandler
	{
		[Inject] IInputListener _inputListener;
		[Inject] IInputData _inputData;

		 public override void Initialize()
		 {
			 _inputListener.TouchChanged += Handler;
		 }
		
		private void Handler(TouchData data)
		{
			if (!Filter())
				return;

			var entities = _inputData.GetTouches();
			if (entities.Count != 2)
				return;

			var inputParametersValue = _inputHandler.InputParameters;

			var touchZero = entities[0];
			var touchOne = entities[1];

			if (touchZero.TouchTarget != null &&
			    touchZero.TouchTarget.layer == LayerMask.NameToLayer("UI"))
				return;

			var touchZeroDelta = touchZero.Touch.Value.deltaPosition;
			var touchOneDelta = touchOne.Touch.Value.deltaPosition;

			if (touchOneDelta.sqrMagnitude < inputParametersValue.zoomThreshold &&
			    touchZeroDelta.sqrMagnitude < inputParametersValue.zoomThreshold)
			{
				return;
			}

			var deltaMagnitudeDiff = CountDelta(touchZero.TouchPosition - touchZeroDelta,
				touchZero.TouchPosition,
				touchOne.TouchPosition - touchOneDelta,
				touchOne.TouchPosition);

			var newOrtho = CalculateNextOrtho(deltaMagnitudeDiff, inputParametersValue);
			Debug.LogError($"DeltaVladOrtho: {newOrtho}");
			_inputHandler.SetZoomTarget(new CameraZoomTarget(newOrtho, 0f, ResetZoom, true));

			touchZero.ReplaceStartTouchPosition(touchZero.TouchPosition);
			touchOne.ReplaceStartTouchPosition(touchOne.TouchPosition);
			_inputHandler.Zooming = true;
		}

		protected override float CalculateNextOrtho(float delta, InputParameters parameters)
		{
			var normalizedDelta = delta / parameters.TouchZoomSensitivity3;
			Debug.LogError($"VladResult1 {delta} / {parameters.TouchZoomSensitivity3} = {normalizedDelta} ");
			Debug.LogError(
				$"VladResult2 {_inputHandler.CameraZoom} - {normalizedDelta} = {_inputHandler.CameraZoom - normalizedDelta} ");
			return _inputHandler.CameraZoom - normalizedDelta;
		}

		private static float CountDelta(Vector2 point1Start, Vector2 point1End, Vector2 point2Start, Vector2 point2End)
		{
			var start = (point2Start - point1Start);
			var startDiff = start.magnitude;
			if (Math.Abs(startDiff) < Mathf.Epsilon)
			{
				start = Vector3.right;
			}

			var end = point2End - point1End;

			return Vector3.Dot(end.normalized, start.normalized) * end.magnitude - startDiff;
		}
	}
}