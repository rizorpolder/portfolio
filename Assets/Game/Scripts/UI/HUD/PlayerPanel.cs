using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Data;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.UI.ResourceTab;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI
{
	public class PlayerPanel : HUDChildPanel
	{
		[SerializeField] private RectTransform tabsRoot;

		[SerializeField] private Image _icon;
		[SerializeField] private Image _background;

		[Inject] private PlayerConfig _playerConfig;
		[Inject] private IPlayerData _playerData;
		[Inject] private IPlayerListener _playerListener;
		private Dictionary<ResourceType, AResourceTab> _resourceTabs;

		private Dictionary<ResourceType, AResourceTab> ResourceTabs
		{
			get
			{
				if (_resourceTabs == null)
				{
					_resourceTabs = tabsRoot.GetComponentsInChildren<AResourceTab>(true)
						.ToDictionary(x => x.Type, x => x);
				}

				return _resourceTabs;
			}
		}

		private void Start()
		{
			_playerListener.OnPlayerDataUpdated += UpdatePlayerView;
			UpdatePlayerView();
		}

		private void UpdatePlayerView()
		{
			
		}

		public bool TryGetResourceTab(ResourceType resourceType, out AResourceTab tab)
		{
			tab = default;
			if (ResourceTabs.ContainsKey(resourceType))
			{
				tab = _resourceTabs[resourceType];
				return true;
			}

			return false;
		}

		public void RebuildTopPanelLayout()
		{
			if (gameObject.activeInHierarchy)
			{
				StartCoroutine(RebuildTopPanelCoroutine());
			}
		}

		public IEnumerator RebuildTopPanelCoroutine()
		{
			yield return new WaitForEndOfFrame();
			LayoutRebuilder.ForceRebuildLayoutImmediate(tabsRoot);
			LayoutRebuilder.MarkLayoutForRebuild(tabsRoot);
		}

		protected override void OnDestroyAction()
		{
			_playerListener.OnPlayerDataUpdated -= UpdatePlayerView;
			base.OnDestroyAction();
		}


	}
}