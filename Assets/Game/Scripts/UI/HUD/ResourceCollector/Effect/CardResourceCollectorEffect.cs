using System;
using DG.Tweening;
using AudioManager.Runtime.Core.Manager;
using UI.HUD.ResourceCollector.Effect;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ResourceCollector
{
	public class CardResourceCollectorEffect : BaseResourceCollectorEffect
	{
		private const float StartAnimationCollectTime = 1f;
		private const float DelayTime = 1f;

		public override void Animate(RectTransform reward,
			Vector3[] curve,
			ResourceCollectTarget finishData,
			Action onComplete)
		{
			var sequence = DOTween.Sequence();
			var image = reward.GetComponent<Image>();
			var temp = image.sprite.rect.size;
			var centerPos = new Vector2(Screen.width / 2, Screen.height / 2);
			sequence.AppendCallback(() =>
			{
				reward.gameObject.SetActive(true);
				//ManagerAudio.SharedInstance.PlayAudioClip(TAudio.count_coins_back.ToString()); //TODO AudioManager
			});
			sequence.Append(reward.DOAnchorPos(centerPos, StartAnimationCollectTime));
			sequence.Join(reward.DOSizeDelta(temp, StartAnimationCollectTime));
			sequence.AppendInterval(DelayTime);
			sequence.Append(reward.DOAnchorPos(finishData.Position, StartAnimationCollectTime));
			sequence.Join(reward.DOSizeDelta(finishData.SizeDelta, StartAnimationCollectTime));

			sequence.OnComplete(() => { onComplete?.Invoke(); });
		}
	}
}