using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.UI.WindowsSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class ZoomAnimator
	{
		private InputHandler _handler;

		private CancellationTokenSource _cancellationTokenSource;
		private Action _onAnimationFinished;
		private bool _isInput;
		private float _startOrtho;
		private float _targetOrtho;
		private float _duration;

		private float _time;

		//TODO
		//public bool IsInputFobbiden => GetGroup(InputMatcher.UserInputFobbiden).count > 0;
		private float _velocity;

		[Inject] private WindowsController _windowsController;

		public ZoomAnimator(InputHandler handler)
		{
			_handler = handler;
		}

		public void Animate()
		{
			var targetOrtho = Mathf.Clamp(_handler.CameraZoomTarget.value,
				_handler.InputParameters.minOrtho,
				_handler.InputParameters.maxOrtho);
			var current = _handler.CameraZoom;

			AddZoom(_handler.CameraZoomTarget.input,
				current,
				targetOrtho,
				_handler.CameraZoomTarget.duration,
				OnZoomEndAction);
		}

		private void OnZoomEndAction()
		{
			_handler.CameraZoomTarget.callback?.Invoke();
			SaveOrthoSize();
		}

		private void SaveOrthoSize()
		{
			if (_isInput)
				PlayerPrefs.SetFloat($"CameraOrthoSize", _handler.CameraZoom);
		}

		public void AddZoom(bool isInput,
			float startOrtho,
			float targetOrtho,
			float duration = 0f,
			Action onComplete = null)
		{
			_isInput = isInput;
			_startOrtho = startOrtho;
			_targetOrtho = targetOrtho;
			_duration = duration;
			_onAnimationFinished += onComplete;
			_time = 0f;

			if (_cancellationTokenSource is null)
			{
				_velocity = 0f;
				_cancellationTokenSource = new CancellationTokenSource();
				Zoom().Forget();
			}
		}

		private async UniTaskVoid Zoom()
		{
			while (true)
			{
				var current = _handler.CameraZoom;
				var size = GetNextOrtho(_handler.Camera);

				_handler.SetCameraZoom(size);
				await UniTask.Yield();

				if (_cancellationTokenSource.IsCancellationRequested)
					break;

				if (_duration == 0f)
				{
					var delta = Mathf.Abs(_handler.CameraZoom - _targetOrtho);
					if (delta < 0.001f)
						break;
				}
				else
				{
					if (_time > _duration)
						break;
				}
			}

			OnEndAnimation();
		}

		private float GetNextOrtho(Camera targetCamera)
		{
			if (_duration == 0f)
			{
				var current = _handler.CameraZoom;
				var smoothTime = _handler.InputParameters.SmoothZoomTime;
				return Mathf.SmoothDamp(current, _targetOrtho, ref _velocity, smoothTime);
			}
			else
			{
				_time += Time.deltaTime;
				return Mathf.SmoothStep(_startOrtho, _targetOrtho, _time / _duration);
			}
		}

		private void OnEndAnimation()
		{
			var isCanceled = _cancellationTokenSource.IsCancellationRequested;
			_cancellationTokenSource = null;
			_startOrtho = 0f;

			if (!isCanceled)
				_onAnimationFinished?.Invoke();

			_onAnimationFinished = null;

			if (!isCanceled && _isInput && !_handler.Zooming)
				TryBounceInBorders();
		}

		private float GetMinInputZoom()
		{
			return _handler.InputParameters.minOrtho + _handler.InputParameters.bounceDeltaZoom;
		}

		private float GetMaxInputZoom()
		{
			return _handler.InputParameters.maxOrtho - _handler.InputParameters.bounceDeltaZoom;
		}

		public void TryBounceInBorders()
		{
			if (_handler.Zooming)
				return;

			// if (Context.Meta.HasImportantAction)
			// 	return;

			if (_windowsController.HasActiveWindows)
				return;

			var current = _handler.CameraZoom;

			if (current >= GetMaxInputZoom())
			{
				AddZoom(false, current, GetMaxInputZoom(), _handler.InputParameters.bounceTimeZoom, null);
			}
			else if (current <= GetMinInputZoom())
			{
				AddZoom(false, current, GetMinInputZoom(), _handler.InputParameters.bounceTimeZoom, null);
			}
		}

		public void KillZoom()
		{
			_cancellationTokenSource?.Cancel();
		}
	}
}