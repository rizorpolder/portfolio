using DG.Tweening;
using Game.Scripts.Systems.Input;
using Game.Scripts.Systems.Input.Config;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.CameraScrollSystem
{
	public class CameraPositionClamping
	{
		private Bounds _cameraAreaBounds;

		[Inject] InputHandler _inputHandler;
		[Inject] InputParameters _inputParameters;

		private Camera _camera;

		public void Initialize()
		{
			_camera = _inputHandler.Camera;
		}

		public void Process()
		{
			var cameraPosition = _camera.transform.position;
			_cameraAreaBounds = _inputParameters.CameraBounds;

			if (_inputHandler.HasStartTouchPosition)
			{
				if (Inside(cameraPosition, out var offset))
					return;

				var hardBounceDistance = _inputParameters.bounceBorderOffset;

				if (Vector3.Distance(cameraPosition, offset) > hardBounceDistance)
				{
					var hardBounds = new Bounds(_cameraAreaBounds.center,
						_cameraAreaBounds.size + _inputParameters.BoundsOffset);
					_camera.transform.position = ApplyBounds(hardBounds, cameraPosition);
				}

				if (_inputHandler.MovementSpeed.magnitude > 0.1f)
					_inputHandler.SetMovementSpeed(Vector2.zero);

				// движение
				cameraPosition = ApplyBounds(_cameraAreaBounds, cameraPosition);
				_inputHandler.SetCameraPosition(cameraPosition);
			}
			else
			{
				cameraPosition = ApplyBounds(_cameraAreaBounds, cameraPosition);
				_inputHandler.SetCameraPosition(cameraPosition);
			}
		}

		public bool Inside(Vector3 position, out Vector3 fixedPosition)
		{
			fixedPosition = position;
			var cameraHeight = _camera.orthographicSize;
			var cameraWidth = (Screen.width * 1f / Screen.height) * cameraHeight;
			var minX = _cameraAreaBounds.min.x + cameraWidth / 2f;
			var maxX = _cameraAreaBounds.max.x - cameraWidth / 2f;
			var maxY = _cameraAreaBounds.max.y - cameraHeight / 2f;
			var minY = _cameraAreaBounds.min.y + cameraHeight / 2f;

			var offset = _inputParameters.bounceBorderOffset;
			fixedPosition.x = Mathf.Max(fixedPosition.x, minX + offset);
			fixedPosition.y = Mathf.Max(fixedPosition.y, minY + offset);
			fixedPosition.x = Mathf.Min(fixedPosition.x, maxX - offset);
			fixedPosition.y = Mathf.Min(fixedPosition.y, maxY - offset);

			var inside = !(position.x < minX ||
			               position.x > maxX ||
			               position.y < minY ||
			               position.y > maxY);
			return inside;
		}

		private Vector3 ApplyBounds(Bounds bounds, Vector3 position)
		{
			var cameraHeight = _camera.orthographicSize;
			var cameraWidth = (Screen.width * 1f / Screen.height) * cameraHeight;
			position.x = Mathf.Max(position.x, bounds.min.x + cameraWidth / 2f);
			position.y = Mathf.Max(position.y, bounds.min.y + cameraHeight / 2f);
			position.x = Mathf.Min(position.x, bounds.max.x - cameraWidth / 2f);
			position.y = Mathf.Min(position.y, bounds.max.y - cameraHeight / 2f);
			return position;
		}
	}
}