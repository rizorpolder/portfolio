using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Data;
using Game.Scripts.Extensions;
using Game.Scripts.UI.Common;
using UI.HUD.ResourceCollector.Effect;
using UnityEngine;
using Zenject;

namespace UI.HUD.ResourceCollector
{
	public class ResourceCollectorBehaviour : MonoBehaviour
	{
		private const int PoolCount = 5;

		#region Curve

		[Header("Curve Config")]
		[SerializeField] private AnimationCurve _curve;

		[SerializeField] private Ease ease;

		[SerializeField, Tooltip("Время анимации полета")]
		private float pathTime;

		[SerializeField, Tooltip("Время изменения размера до размера цели")]
		private float sizeTime;

		[SerializeField, Tooltip("Задержка между анимациями объектов")]
		private float animationStartDelay = 0.03f;

		[SerializeField, Tooltip("Время перехода альфы из 0 в 1 на старте")]
		private float TimeToRewardFadeIn = 0.17f;

		[SerializeField,
		 Tooltip("Время перехода альфы из 1 в 0 на на финише (вычитается из времени пути)")]
		private float TimeToRewardFadeOut = 0.17f;

		#endregion

		#region Punch

		[Space]
		[Header("Target Punch Config")]
		[SerializeField] private Vector3 punchScale;

		[SerializeField] private float punchDuration;

		[Space]

		#endregion

		[SerializeField] private ResourceList ResourceList;

		[SerializeField] public List<ResourceCollectTargetData> resourceIcons;
		[SerializeField] private Transform root;
		
		
		public Transform Root => root;

		private Dictionary<ResourceType, Queue<PoolObjectWrapper>> _iconsPool;
		private Dictionary<ResourceType, ResourceCollectTargetData> _resourceIconsCache;
		private Dictionary<ResourceType, int> _flyingQueues = new Dictionary<ResourceType, int>();
		private bool _isNeedReTarget = false;
		private int defaultCanvasLayer = 3;
		private Sequence _punchSequence;

		[Inject] private GlobalUI _globalUI;
		
		
		private void Awake()
		{
			_resourceIconsCache = resourceIcons.ToDictionary(x => x.resourceType, x => x);
			_iconsPool = new Dictionary<ResourceType, Queue<PoolObjectWrapper>>();
			foreach (var resourceViewData in resourceIcons)
			{
				var resourceType = resourceViewData.resourceType;
				if (_iconsPool.ContainsKey(resourceType) || !resourceViewData.resourceIconPrefab)
					continue;
				_iconsPool.Add(resourceType, new Queue<PoolObjectWrapper>());

				// TODO: one object with image
				for (var i = 0; i < PoolCount; i++)
				{
					SpawnResource(resourceType, resourceViewData);
				}
			}
		}

		public UniTask CollectResource(CollectResourceAnimationData data)
		{
			var resourceCount = GetResourceCount(data.resource);
			if (resourceCount == 0 || !TryGetTargetResourceData(data, out var finishData))
			{
				return UniTask.CompletedTask;
			}

			if (!data.isChangeSize)
				finishData.SizeDelta = data.start.SizeDelta;

			data.SetFinish(finishData);

			return AnimateResource(data);
		}

		public UniTask SpendResource(CollectResourceAnimationData data)
		{
			var resourceCount = GetResourceCount(data.resource);

			if (resourceCount == 0 || !TryGetTargetResourceData(data, out var startData))
			{
				return UniTask.CompletedTask;
			}

			data.SetFinish(data.start);
			data.start = startData;

			return AnimateResource(data);
		}

		private UniTask AnimateResource(CollectResourceAnimationData data)
		{
			var animation = new ResourceCollectorAnimation();
			StartCoroutine(AnimateResourceEnumerator(data, animation));
			return UniTask.WaitUntil(() => animation.Completed);
		}

		private IEnumerator AnimateResourceEnumerator(CollectResourceAnimationData data,
			ResourceCollectorAnimation resAnimation)
		{
			var isCoins = data.resource.Type == ResourceType.Points;
			if (!TryGetResourceIconPrefab(data.resource.Type, out var queue))
			{
				yield break;
			}

			ResourceCollectTarget startData = data.start;
			ResourceCollectTarget finishData = default;
			// custom finish target
			if (!data.finish.HasValue)
			{
				yield return ShowHudPanel(data.resource.Type);
				if (_isNeedReTarget)
				{
					TryGetTargetResourceData(data, out finishData);
				}

				_isNeedReTarget = false;
			}
			else
			{
				finishData = data.finish.Value;
			}

			var resourceCount = GetResourceCount(data.resource);


			yield return ShowHudPanel(data.resource.Type, true);

			if (!_flyingQueues.ContainsKey(data.resource.Type))
				_flyingQueues.Add(data.resource.Type, 1);
			else
				_flyingQueues[data.resource.Type]++;

			var hasResourceTab =
				_globalUI.HUD.PlayerPanel.TryGetResourceTab(data.resource.Type, out var resourceTab);
			var resourceDelta = (int) data.resource.Value / resourceCount;

			var newCurve = CreateCurve(data.start.GetActualPosition(), finishData.GetActualPosition());

			resourceCount = Math.Abs(resourceCount);

			for (var i = 0; i < resourceCount; i++)
			{
				var startPosition = startData.GetActualPosition();

				if (queue.Count == 0)
				{
					SpawnResource(data.resource.Type, _resourceIconsCache[data.resource.Type]);
				}

				var reward = queue.Dequeue();
				reward.canvas.sortingOrder = defaultCanvasLayer + i;
				var rewardStartData = reward.rectTransform.GetResourceCollectTargetData();

				reward.rectTransform.SetAsFirstSibling();
				reward.rectTransform.anchoredPosition = startPosition;
				reward.rectTransform.gameObject.SetActive(i == resourceCount - 1);

				if (reward.rectTransform)
				{
					reward.rectTransform.sizeDelta = startData.SizeDelta;
				}

				reward.rectTransform.rotation = startData.Rotation;

				var iterator = i;
				GetEffect(data.resource)
					.SetAnimationDelay(animationStartDelay * i)
					.SetPathDuration(pathTime)
					.SetSizeDuration(sizeTime)
					.SetRewardFadeInTime(TimeToRewardFadeIn)
					.SetRewardFadeOutTime(TimeToRewardFadeOut)
					.SetEaseType(ease)
					.Animate(reward.rectTransform,
						newCurve,
						finishData,
						() =>
						{
							reward.rectTransform.ApplyTargetResourceCollectData(rewardStartData);
							reward.rectTransform.gameObject.SetActive(false);
							queue.Enqueue(reward);

							// if (isCoins)
							// 	ManagerAudio.SharedInstance.PlayAudioClip(TAudio.CoinReachSound.ToString()); //TODO AudioManager

							if (hasResourceTab && !resAnimation.Completed)
							{
								resourceTab.ChangeCounter(resourceDelta);
							}

							if (iterator == resourceCount - 1)
							{
								_flyingQueues[data.resource.Type]--;
								resAnimation.Complete();
								HideHudPanel(data.resource);
							}

							var iconTransform = _resourceIconsCache[data.resource.Type].image.transform;
							_punchSequence = DOTween.Sequence();
							_punchSequence.Append(
								iconTransform.DOScale(punchScale, punchDuration).SetEase(Ease.InCubic));
							_punchSequence.Append(iconTransform.DOScale(1f, 0.1f).SetEase(Ease.OutCubic));
							_punchSequence.Play();
						});
			}

			//ManagerAudio.SharedInstance.PlayAudioClip(TAudio.count_coins_back.ToString()); //TODO AudioManager
		}

		private IResourceCollectorEffect GetEffect(Resource resource)
		{
			return new BaseResourceCollectorEffect();
		}

		private bool TryGetResourceIconPrefab(ResourceType resourceType, out Queue<PoolObjectWrapper> resourcePool)
		{
			resourcePool = new Queue<PoolObjectWrapper>();
			if (!_iconsPool.ContainsKey(resourceType))
				return false;
			resourcePool = _iconsPool[resourceType];
			return true;
		}

		private bool TryGetTargetResourceData(CollectResourceAnimationData data, out ResourceCollectTarget targetData)
		{
			targetData = default;

			if (data.finish.HasValue)
			{
				targetData = data.finish.Value;
				return true;
			}

			if (!_resourceIconsCache.ContainsKey(data.resource.Type) || !_resourceIconsCache[data.resource.Type].root)
				return false;

			targetData = _resourceIconsCache[data.resource.Type].root.GetResourceCollectTargetData();
			return true;
		}

		private int GetResourceCount(Resource resource)
		{
			var resourceCount = (int) resource.Type;
			switch (resource.Type)
			{
				case ResourceType.Points:
				{
					if (resourceCount <= 2)
						return 1;
					else if (resourceCount <= 4)
						return 2;
					else if (resourceCount <= 10)
						return 4;
					else
						return 8;
				}
					;
				default:
				{
					return Math.Min((int) resource.Value, PoolCount);
				}
			}
		}

		private void SpawnResource(ResourceType resourceType, ResourceCollectTargetData resourceViewData)
		{
			var resourceCollectObj = Instantiate(resourceViewData.resourceIconPrefab, root);
			resourceCollectObj.SetActive(false);
			var rt = resourceCollectObj.GetComponent<RectTransform>();
			var c = resourceCollectObj.GetComponent<Canvas>();
			_iconsPool[resourceType].Enqueue(new PoolObjectWrapper(rt, c));
		}

		private IEnumerator ShowHudPanel(ResourceType resourceType, bool waitAnimation = false)
		{
			if (_resourceIconsCache.ContainsKey(resourceType))
			{
				bool isNeedRebuild = _resourceIconsCache[resourceType].hud.IsHidden();
				_resourceIconsCache[resourceType].hud.Show();

				if (isNeedRebuild)
				{
					if (waitAnimation)
						yield return new WaitForSeconds(0.15f);

					yield return _globalUI.HUD.PlayerPanel.RebuildTopPanelCoroutine();
					_isNeedReTarget = true;
				}
			}
		}

		private async void HideHudPanel(Resource resource)
		{
			int delay = resource.Value > 0 ? 200 : 1000;
			await UniTask.Delay(delay);

			if (_flyingQueues[resource.Type] > 0)
				return;

			if (_resourceIconsCache.ContainsKey(resource.Type))
			{
				_resourceIconsCache[resource.Type].hud.Hide(Hud_OnHiddenAction);
			}
		}

		private void Hud_OnHiddenAction()
		{
			//_globalUI.HUD.TopPanel.RebuildTopPanelLayout();
		}

		public Vector3[] CreateCurve(Vector3 fromPos, Vector3 toPos)
		{
			var keys = _curve.keys;

			var distance = toPos - fromPos;

			var sign = toPos.x > fromPos.x ? (toPos.y > fromPos.y ? 1 : -1) : (toPos.y > fromPos.y ? -1 : 1);

			Vector3[] resultArr = {fromPos};
			for (var index = 1; index < keys.Length - 1; index++)
			{
				var key = keys[index];
				var curvePoint = distance * key.time;
				Vector3 perpendicular = Vector2.Perpendicular(curvePoint) * sign;
				var resultPoint = fromPos + curvePoint;
				var point = resultPoint + (perpendicular * key.value);
				resultArr = resultArr.Add(point);
			}

			resultArr = resultArr.Add(toPos);
			return resultArr;
		}

		[Serializable]
		private class PoolObjectWrapper
		{
			public RectTransform rectTransform;
			public Canvas canvas;

			public PoolObjectWrapper(RectTransform rectTransform, Canvas canvas)
			{
				this.rectTransform = rectTransform;
				this.canvas = canvas;
			}
		}
	}
}