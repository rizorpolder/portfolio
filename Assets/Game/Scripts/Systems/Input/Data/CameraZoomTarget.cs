using System;

namespace Game.Scripts.Systems.Input.Data
{
	public class CameraZoomTarget
	{
		public float value;
		public float duration;
		public Action callback;
		public bool input;

		public CameraZoomTarget(float val, float durationSeconds, Action completionCallback, bool isInput)
		{
			value = val;
			duration = durationSeconds;
			callback = completionCallback;
			input = isInput;
		}
	}
}