using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Common.UI
{
	public class UIFiller : MonoBehaviour
	{
		[SerializeField] private RectTransform.Axis fillerDirection = RectTransform.Axis.Horizontal;
		[SerializeField] protected RectTransform progressRect;
		[SerializeField] protected Image fillerImage;

		[SerializeField] private float _progressFullWidth;
		[SerializeField] private float _progressFullHeight;
		private float _defaultWidth;
		private float _defaultHeight;

		private void OnValidate()
		{
			if (_progressFullWidth == 0)
				_defaultWidth = ProgressFullWidth;

			if (_progressFullHeight == 0)
				_defaultHeight = ProgressFullHeight;
		}

		public float ProgressFullWidth
		{
			get
			{
				if (_progressFullWidth == 0)
				{
					DefaultWidth = _progressFullWidth = progressRect.rect.width;
				}

				return _progressFullWidth;
			}
			set { _progressFullWidth = value; }
		}

		public float ProgressFullHeight
		{
			get
			{
				if (_progressFullHeight == 0)
				{
					DefaultHeight = _progressFullHeight = progressRect.rect.height;
				}

				return _progressFullHeight;
			}
			set { _progressFullHeight = value; }
		}

		public RectTransform ProgressRect
		{
			get => progressRect;
		}

		public float DefaultWidth
		{
			get => _defaultWidth;
			set => _defaultWidth = value;
		}

		public float DefaultHeight
		{
			get => _defaultHeight;
			set => _defaultHeight = value;
		}

		public virtual void SetFillerValue(float value)
		{
			var progress = fillerDirection == RectTransform.Axis.Horizontal ? ProgressFullWidth : ProgressFullHeight;
			var currentFillerSize = progress * value;

			progressRect.SetSizeWithCurrentAnchors(fillerDirection, currentFillerSize);
		}

		public void SetFillerSprite(Sprite sprite)
		{
			fillerImage.sprite = sprite;
		}

		/// <summary>
		/// Animated Filling from parameter value
		/// </summary>
		/// <param name="startValue"></param>
		/// <param name="endValue"></param>
		/// <param name="animationTime"></param>
		/// <returns></returns>
		public Tween SetFillerValueAnimated(float startValue, float endValue, float animationTime)
		{
			return DOTween.To(() => startValue, x => startValue = x, CheckEndValue(endValue), animationTime)
				.OnUpdate(() => { SetFillerValue(startValue); });
		}

		/// <summary>
		/// Animated Filling from current filled value
		/// </summary>
		/// <param name="endValue"></param>
		/// <param name="animationTime"></param>
		/// <returns></returns>
		public Tween SetFillerValueAnimated(float endValue, float animationTime)
		{
			var progress = fillerDirection == RectTransform.Axis.Horizontal ? ProgressFullWidth : ProgressFullHeight;
			var currentValue = progressRect.rect.size.x / progress;
			return DOTween.To(() => currentValue, x => currentValue = x, CheckEndValue(endValue), animationTime)
				.OnUpdate(() => { SetFillerValue(currentValue); });
		}

		private float CheckEndValue(float value)
		{
#if UNITY_EDITOR
			if (value > 1)
			{
				Debug.Log($"Progress Bar {gameObject.name} value");
			}
#endif
			return value > 1 ? 1 : value;
		}
	}
}