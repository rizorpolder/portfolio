using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Input;
using Game.Scripts.Systems.Input.Data;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.CameraScrollSystem
{
	public class CameraScrollHandler 
	{
		private Vector3 _velocity = Vector3.zero;
		private Vector3 _target;

		private Vector3 _startCameraPosition;
		private Vector3 _delta;
		private bool _scrollInProcess;

		private CancellationTokenSource _cancellationTokenSource;

		[Inject] private InputHandler _inputHandler;
		[Inject] private IInputListener _inputListener;
		[Inject] private IInputData _inputData;

		public void Initialize()
		{
			_inputListener.ScrollDataChanged += ScrollDataChangedHandler;
		}
		
		
		private void ScrollDataChangedHandler(TouchData data)
		{
			if (IsBlocked(data))
			{
				CancelScroll();
				return;
			}

			//touches delta from touch's start to current position
			var camera = _inputHandler.Camera;
			var delta = camera.ScreenToWorldPoint(data.TouchPosition) -
			            camera.ScreenToWorldPoint(data.StartTouchPosition);

			//save touch start position to the camera's entity if the touch just started to mark camera as being moved
			if (data.isTouchDown)
				_inputHandler.ReplaceStartTouchPosition(camera.transform.position);

			Scroll(delta);
		}

		private bool IsBlocked(TouchData data)
		{
			if (!_inputData.IsSingleTouch)
				return true;
			
			if (data.isTouchUp)
				return true;

			if (data.TouchTarget != null && data.TouchTarget.layer == LayerMask.NameToLayer("UI"))
				return true;

			return false;
		}

		private void Scroll(Vector3 delta)
		{
			if (!_scrollInProcess)
			{
				_startCameraPosition = _inputHandler.Camera.transform.position;
				_scrollInProcess = true;
			}

			_delta = delta;
			_target = _startCameraPosition - _delta;
			_velocity = Vector3.zero;

			if (_cancellationTokenSource is not null)
				return;

			_cancellationTokenSource = new CancellationTokenSource();
			MoveSmooth(_cancellationTokenSource).Forget();
		}

		private async UniTaskVoid MoveSmooth(CancellationTokenSource cancellationTokenSource)
		{
			while (true)
			{
				var current = _inputHandler.Camera.transform.position;
				var scrollSpeed = _inputHandler.InputParameters.ScrollSpeed;
				var next = Vector3.SmoothDamp(current, _target, ref _velocity, scrollSpeed);
				_inputHandler.SetCameraPosition(next);
				await UniTask.Yield();

				if (cancellationTokenSource.IsCancellationRequested)
					break;
			}
		}

		private void CancelScroll()
		{
			_target = Vector2.zero;
			_delta = Vector2.zero;
			_scrollInProcess = false;
			CancelScrollToken();
		}

		private void CancelScrollToken()
		{
			if (_cancellationTokenSource is null)
				return;

			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
			_cancellationTokenSource = null;
		}

		public void Dispose()
		{
			_inputListener.ScrollDataChanged -= ScrollDataChangedHandler;
			CancelScrollToken();
		}


	}
}