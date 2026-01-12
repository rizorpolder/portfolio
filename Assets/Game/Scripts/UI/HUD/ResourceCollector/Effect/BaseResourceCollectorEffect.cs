using DG.Tweening;
using System;
using Game.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ResourceCollector.Effect
{
	public class BaseResourceCollectorEffect : IResourceCollectorEffect
	{
		private float _animationPathTime = 0.65f;
		private float _animationSizeTime = 0.7f;
		private float _timeFadeIn = 0.17f;
		private float _timeFadeOut = 0.17f;
		private float _animationDelay = 0.1f;
		private Ease _ease;

		public virtual void Animate(RectTransform reward,
			Vector3[] curve,
			ResourceCollectTarget finishData,
			Action onComplete)
		{
			var sequence = DOTween.Sequence();
			var image = reward.GetComponent<Image>();

			sequence.AppendInterval(_animationDelay);
			sequence.AppendCallback(() => { reward.gameObject.SetActive(true); });

			sequence.Append(reward.DOAnchoredPath(curve,
				_animationPathTime,
				PathType.CatmullRom,
				PathMode.Ignore,
				20).SetEase(_ease));

			sequence.Join(reward.DOSizeDelta(finishData.SizeDelta, _animationSizeTime));
			sequence.Join(reward.DORotate(Vector3.zero, _animationSizeTime));

			reward.GetComponent<Image>().color = new Color(1, 1, 1, 0);

			sequence.Insert(0, image.DOFade(1, _timeFadeIn));

			var sequenceDuration = sequence.Duration();
			sequence.Insert(sequenceDuration - _timeFadeOut, image.DOFade(0, _timeFadeOut));

			sequence.OnComplete(() => { onComplete?.Invoke(); });
		}

		public IResourceCollectorEffect SetEaseType(Ease ease)
		{
			_ease = ease;
			return this;
		}

		public IResourceCollectorEffect SetPathDuration(float duration)
		{
			_animationPathTime = duration;
			return this;
		}

		public IResourceCollectorEffect SetSizeDuration(float duration)
		{
			_animationSizeTime = duration;
			return this;
		}

		public IResourceCollectorEffect SetRewardFadeOutTime(float duration)
		{
			_timeFadeOut = duration;
			return this;
		}

		public IResourceCollectorEffect SetRewardFadeInTime(float duration)
		{
			_timeFadeIn = duration;
			return this;
		}

		public IResourceCollectorEffect SetAnimationDelay(float duration)
		{
			_animationDelay = duration;
			return this;
		}
	}
}