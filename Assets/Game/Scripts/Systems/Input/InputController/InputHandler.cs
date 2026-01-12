using Game.Scripts.Systems.Input.Config;
using Game.Scripts.Systems.Input.Data;
using Game.Scripts.UI.Common;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class InputHandler
	{
		private readonly ZoomAnimator _zoomAnimator;

		private InputParameters _inputParameters;
		public InputParameters InputParameters => _inputParameters;

		private Camera _camera;

		public Camera Camera
		{
			get
			{
				if (!_camera)
				{
					_camera = _globalUI.HasCamera ? _globalUI.Camera : Camera.main;
				}

				return _camera;
			}
		}

		[Inject] private GlobalUI _globalUI;

		[Inject]
		public InputHandler(DiContainer diContainer, InputParameters inputParameters)
		{
			_inputParameters = inputParameters;
			_zoomAnimator = new ZoomAnimator(this);
			diContainer.Inject(_zoomAnimator);
		}

		#region Camera Position

		private Vector3 _centerViewportPos;
		public Vector3 CenterViewPortPosition => _centerViewportPos;

		private Vector3 _camPos;
		public Vector3 CameraPosition => _camPos;

		public void SetCameraPosition(Vector3 pos)
		{
			var oldCameraPos = Camera.transform.position;
			_camPos = pos;
			_camPos.z = oldCameraPos.z;
			Camera.transform.position = _camPos;
			SetCenterViewPortPosition(_camPos);
		}

		public void SetCenterViewPortPosition(Vector3 pos)
		{
			_centerViewportPos = pos;
		}

		#endregion

		#region CameraMovement

		private Vector2? _movementSpeed;
		public Vector2 MovementSpeed => HasMovementSpeed ? _movementSpeed.Value : Vector2.zero;
		public bool HasMovementSpeed => _movementSpeed.HasValue;

		public void SetMovementSpeed(Vector2 speed)
		{
			_movementSpeed = speed;
		}

		#endregion

		#region Camera Last Position

		private Vector3? _lastPosition;
		public Vector3 LastPosition => _lastPosition.HasValue ? _lastPosition.Value : Vector3.zero;
		public bool HasLastPosition => _lastPosition.HasValue;

		public void SetLastPosition(Vector3 pos)
		{
			_lastPosition = pos;
		}

		#endregion

		#region Camera Zoom

		private float _camZoom = 9.5f; //todo
		public float CameraZoom => _camZoom;

		public void SetCameraZoom(float zoom)
		{
			_camZoom = zoom;

			Camera.orthographicSize = _camZoom;
		}

		private CameraZoomTarget _zoomTarget;
		public CameraZoomTarget CameraZoomTarget => _zoomTarget;

		public void SetZoomTarget(CameraZoomTarget target)
		{
			_zoomTarget = target;
			_zoomAnimator.Animate();
		}

		private bool _zooming;

		public bool Zooming
		{
			get => _zooming;
			set
			{
				if (_zooming != value)
				{
					_zooming = value;
					if (!_zooming)
						_zoomAnimator.TryBounceInBorders();
				}
			}
		}

		#endregion

		private Vector3? _startTouchPosition;
		public Vector3 StartTouchPosition => _startTouchPosition.HasValue ? _startTouchPosition.Value : Vector3.zero;
		public bool HasStartTouchPosition => _startTouchPosition.HasValue;

		public void ReplaceStartTouchPosition(Vector3 pos)
		{
			_startTouchPosition = pos;
		}

		public void RemoveStartTouchPosition()
		{
			_startTouchPosition = null;
		}

		public void ProcessInertia()
		{
			if (!HasMovementSpeed || HasStartTouchPosition)
				return;

			var speed = MovementSpeed;
			if (speed.Equals(Vector3.zero))
				return;

			var newSpeed = Vector3.Lerp(speed, Vector2.zero, InputParameters.cameraInertiaRate * Time.deltaTime);

			var cameraPosition = Camera.transform.position;

			var newPosition = cameraPosition + Time.deltaTime * newSpeed;

			SetCameraPosition(newPosition);
			SetMovementSpeed(newSpeed);
		}

		public void ProcessCameraSpeed()
		{
			var currentCameraPosition = Camera.transform.position;

			if (HasLastPosition && HasStartTouchPosition)
			{
				var dist = Vector3.Distance(LastPosition, currentCameraPosition);

				Vector2 speed = (currentCameraPosition - LastPosition) / Time.deltaTime;

				SetMovementSpeed(speed);
			}

			SetLastPosition(currentCameraPosition);
		}
	}
}