using UnityEngine;

namespace Game.Scripts.Systems.Input.Config
{
	[CreateAssetMenu(fileName = "InputParameters", menuName = "Project/Input/Parameters")]
	public class InputParameters : ScriptableObject
	{
		public float ScrollSpeed = 0.08f;
		public float MouseZoomSensitivity = 1;
		public float TouchZoomSensitivity3 = 30f;
		public float SmoothZoomTime = 0.1f;

		[Tooltip("Минимальная высота камеры")]
		public float minOrtho = 3;

		[Tooltip("Максимальная высота камеры")]
		public float maxOrtho = 6;

		public float zoomThreshold = 0.05f;

		public readonly float cameraInertiaRate = 3f;

		[Tooltip("Время отскока при движении за границы.")]
		public float bounceTimeClamping = 0.5f;

		[Tooltip("Отступ от границы для баунса при движении камеры.")]
		public float bounceBorderOffset = 1f;

		[Tooltip("Время отскока при зуммировании.")]
		public float bounceTimeZoom = 0.5f;

		[Tooltip("Сила отскока при зуммировании.")]
		public float bounceDeltaZoom = 0.1f;

		[Tooltip("Размер границы фиксации камеры")]
		public Bounds CameraBounds;

		public Vector3 BoundsOffset;
		
	}
}