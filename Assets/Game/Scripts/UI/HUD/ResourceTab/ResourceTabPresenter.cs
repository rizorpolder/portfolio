using DG.Tweening;
using Game.Scripts.Data;
using Game.Scripts.UI.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.ResourceTab
{
	public class ResourceTabPresenter : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI countText;
		[SerializeField] private Image icon;

		private Sequence _counterAnimation;
		private int _currentCount;

		[Inject] private ResourcesSpriteRepository _resourcesSpriteRepository;

		public virtual void Refresh(Resource resource, float progress)
		{
			_currentCount = (int) resource.Value;

			RefreshIcon(resource);
			SetCounter(_currentCount);
		}

		public void RefreshIcon(Resource resource)
		{
			if (icon)
			{
				icon.sprite = _resourcesSpriteRepository.GetResourceSprite(resource);
			}
		}

		public void ChangeCounter(int delta)
		{
			_currentCount += delta;
			SetCounter(_currentCount);
		}

		private void SetCounter(int final)
		{
			if (!countText) return;

			_counterAnimation?.Kill(true);
			countText.text = GetFormatCounter(final);
		}

		private string GetFormatCounter(int count)
		{
			return count.ToString();
		}

		public void SetActiveCounterText(bool isEnabled)
		{
			countText.gameObject.SetActive(isEnabled);
		}
	}
}