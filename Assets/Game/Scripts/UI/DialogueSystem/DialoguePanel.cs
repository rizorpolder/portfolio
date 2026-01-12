using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.UI.Common;
using UnityEngine;

namespace Game.Scripts.UI.DialogueSystem
{
	public class DialoguePanel : MonoBehaviour
	{
		[SerializeField] private DialogueCharacterPanel _panelPC;
		[SerializeField] private DialogueCharacterPanel _panelNPC;

		[SerializeField] private DialogueTextPanel _textPanel;

		private bool opened = false;
		public bool IsTyping => _textPanel.IsTyping;
		public void Init()
		{
			gameObject.SetActive(false);
			_textPanel.Initialize();
		}

		void PreOpen()
		{
			_panelPC.gameObject.SetActive(false);
			_panelNPC.gameObject.SetActive(false);
		}

		public void Open()
		{
			gameObject.SetActive(true);
			PreOpen();
			opened = true;
		}

		public void Close()
		{
			opened = false;

			bool deferredclose = false;
			if (!_panelPC.Status.Equals(ElementStatus.Hidden))
			{
				deferredclose = true;
				_panelPC.Hide(OnClose);
				_textPanel.Hide();
			}

			if (!_panelNPC.Status.Equals(ElementStatus.Hidden))
			{
				deferredclose = true;
				_panelNPC.Hide(OnClose);
				_textPanel.Hide();
			}

			if (!deferredclose)
				OnClose();
		}

		private void OnClose()
		{
			if (!opened)
			{
				_panelPC.Close();
				_panelNPC.Close();
				gameObject.SetActive(false);
			}
		}

		public void Show(Subtitle subtitle)
		{
			if (subtitle == null)
			{
				_panelPC.Hide();
				_panelNPC.Hide();
				_textPanel.Hide();
				return;
			}

			bool hasText = !string.IsNullOrEmpty(subtitle.FormattedText); // есть ли текст
			bool hasSpeaker = !string.IsNullOrEmpty(subtitle.SpeakerInfo.Name);
			bool hasListener = !string.IsNullOrEmpty(subtitle.ListenerInfo.Name);
		


			if (!hasSpeaker)
				_panelNPC.Hide();

			if (!hasListener)
				_panelPC.Hide();
			

			if (hasText && hasSpeaker)
			{
				_panelPC.SetContent(subtitle);
				_textPanel.SetContent(subtitle);
				if (hasListener)
				{
					_panelNPC.SetContent(subtitle);
				}
			}
		}

		public void Hide()
		{
		}

		public bool TrySkipTypewriter()
		{
			if (!_textPanel.IsTyping)
				return false;

			_textPanel.SkipTypewriterEffect();
			return true;
		}
	}
}