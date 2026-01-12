using Game.Scripts.Common.Pool;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.Texts;
using Game.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.DialogueSystem
{
	public class DialogueTextPanel : BasePanel
	{
		[SerializeField] private RectTransform _contentRoot;

		[SerializeField] private DialogueTextPanelEntry _panelNPC;
		[SerializeField] private DialogueTextPanelEntry _panelPlayer;
		[SerializeField] private ScrollRect _scrollRect;

		[Inject] private ObjectsPoolFactory _objectsPoolFactory;
		[Inject] private TextsValidationSystem _textsValidationSystem;
		private ObjectsPool<DialogueTextPanelEntry> _npcPanelsPool;
		private ObjectsPool<DialogueTextPanelEntry> _playerPanelsPool;

		private DialogueTextPanelEntry _lastCalledPanel;

		public bool IsTyping => _lastCalledPanel && _lastCalledPanel.IsTyping;

		public void Initialize()
		{
			_npcPanelsPool = _objectsPoolFactory.CreatePool(_panelNPC, _contentRoot);
			_playerPanelsPool = _objectsPoolFactory.CreatePool(_panelPlayer, _contentRoot);
		}

		public void SetContent(Subtitle subtitle)
		{
			Show();

			var pool = subtitle.SpeakerInfo.IsPlayer ? _playerPanelsPool : _npcPanelsPool;
			var entryPanel = pool.GetItem();
			entryPanel.transform.SetAsLastSibling();
			_contentRoot.anchoredPosition = new Vector2(_contentRoot.anchoredPosition.x, 0);
			if (_lastCalledPanel)
			{
				_lastCalledPanel.SkipTypewriterEffect();
				_lastCalledPanel.SetState(false);
			}

			_lastCalledPanel = entryPanel;
			entryPanel.SetState(true);
			entryPanel.SetText(_textsValidationSystem.GetLocalizedString(subtitle.FormattedText));
		}

		protected override void OnHideAction()
		{
			ClearPool();
		}

		private void ClearPool()
		{
			foreach (var activeItem in _playerPanelsPool.GetActiveItems())
			{
				activeItem.Release();
			}

			_playerPanelsPool.ResetPool();

			foreach (var activeItem in _npcPanelsPool.GetActiveItems())
			{
				activeItem.Release();
			}

			_npcPanelsPool.ResetPool();

			_lastCalledPanel = null;
		}

		public void SkipTypewriterEffect()
		{
			_lastCalledPanel?.SkipTypewriterEffect();
		}

		public void SetScrollable(bool flag)
		{
			if (_scrollRect.vertical == flag)
				return;

			_scrollRect.vertical = flag;
		}
	}
}