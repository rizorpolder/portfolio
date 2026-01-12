using DG.Tweening;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Common.Animation
{
	public class ShadowAnimation : BaseAnimation
	{
		[Header("Shadow parameters:")]
		[SerializeField]
		private Color _activeColor;

		[SerializeField] private Color _noActiveColor;

		[SerializeField] private float _timeToShow;
		[SerializeField] private float _timeToHide;
		[SerializeField] private float _timeDelayShow;
		[SerializeField] private float _timeDelayHide;

		[SerializeField] private AnimationCurve _curve;
		[SerializeField] private Image _image;
		[SerializeField] private GraphicRaycaster _raycaster;
		[SerializeField] private Canvas _shadowCanvas;
		private Color _currentColor;

		protected override void OnAwake()
		{
		}

		protected override void OnStart()
		{
		}

		private void Reset()
		{
			if (_image == null)
				_image = GetComponent<Image>();
		}

		public void ResetColor()
		{
			_currentColor = _activeColor;
		}

		public void SetActiveColor(Color color)
		{
			_currentColor = color;
		}

		public override UniTask Show(PostAnimationAction action = null)
		{
			gameObject.SetActive(true);
			_image.DOKill();
			_image.DOColor(_currentColor, _timeToShow).SetEase(_curve).SetDelay(_timeDelayShow).Play().OnComplete(() =>
			{
				action?.Invoke();
			});

			return UniTask.CompletedTask;
		}

		public override UniTask Hide(PostAnimationAction action = null)
		{
			if (!gameObject.activeSelf)
			{
				ResetColor();
				return UniTask.CompletedTask;
				;
			}

			_image.DOKill();
			_image.DOColor(_noActiveColor, _timeToHide).SetEase(_curve).SetDelay(_timeDelayHide).Play().OnComplete(() =>
			{
				action?.Invoke();
				gameObject.SetActive(false);
				ResetColor();
			});

			return UniTask.CompletedTask;
			;
		}

		public override UniTask Play(string name, PostAnimationAction action = null)
		{
#if UNITY_EDITOR
			Debug.LogErrorFormat("ShadowAnimation has not implemented method Play");
#endif
			return UniTask.CompletedTask;
		}

		public void ChangeCanvasState(bool isEnabled)
		{
			if (!_shadowCanvas)
				return;

			// hack override not changed when object is disabled
			bool isHidden = !gameObject.activeSelf;
			if (isHidden)
				gameObject.SetActive(true);

			_shadowCanvas.overrideSorting = isEnabled;

			if (isHidden)
				gameObject.SetActive(false);

			if (!isEnabled)
				return;

			_shadowCanvas.sortingOrder = -1;
			_shadowCanvas.sortingLayerName = "UI";
		}
	}
}