using System;
using DG.Tweening;
using UnityEngine;

namespace UI.HUD.ResourceCollector.Effect
{
    public interface IResourceCollectorEffect
    {
        public void Animate(RectTransform reward, Vector3[] curve, ResourceCollectTarget finishData, Action onComplete);
        public IResourceCollectorEffect SetEaseType(Ease ease);
        public IResourceCollectorEffect SetPathDuration(float duration);
        public IResourceCollectorEffect SetSizeDuration(float duration);
        public IResourceCollectorEffect SetRewardFadeOutTime(float duration);
        public IResourceCollectorEffect SetRewardFadeInTime(float duration);
        public IResourceCollectorEffect SetAnimationDelay(float duration);
    }
}
