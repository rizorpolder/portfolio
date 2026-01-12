using System;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.DialogueSystem.Events;
using Game.Scripts.UI.DialogueSystem;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class ConversationView
	{
		public event EventHandler<SelectedResponseEventArgs> SelectedResponseHandler = null;

		public event EventHandler FinishedSubtitleHandler = null;

		private DialogueUI _dialogueUI;
		private Subtitle _subtitle = null;

		private bool _notifyOnFinishSubtitle = false;
		private bool _waitForContinue = false;

		public void Initialize(DialogueUI dialogueUI)
		{
			_dialogueUI = dialogueUI;
			dialogueUI.Open();
			dialogueUI.SelectedResponseHandler += OnSelectedResponse;
			dialogueUI.OnContinueEvent += OnConversationContinue;
		}

		private void OnConversationContinue()
		{
			HandleContinueButtonClick();
		}

		private void HandleContinueButtonClick()
		{
			_waitForContinue = false;
			FinishSubtitle();
		}

		public void Close()
		{
			_dialogueUI.SelectedResponseHandler -= OnSelectedResponse;
			_dialogueUI.OnContinueEvent -= OnConversationContinue;
			_dialogueUI.Close();
			
		}

		public void StartSubtitle(Subtitle subtitle)
		{
			_notifyOnFinishSubtitle = true;
			if (subtitle != null)
			{
				_dialogueUI.ShowSubtitle(subtitle);
				_subtitle = subtitle;
			}
			else
			{
				FinishSubtitle();
			}
		}

		private void OnSelectedResponse(object sender, SelectedResponseEventArgs e)
		{
			SelectResponse(e);
		}

		public void SelectResponse(SelectedResponseEventArgs e)
		{
			_dialogueUI.HideResponses();
			if (SelectedResponseHandler != null)
				SelectedResponseHandler(this, e);
		}

		private void FinishSubtitle()
		{
			if (!_waitForContinue)
			{
				_dialogueUI.HideSubtitle();
				if (_notifyOnFinishSubtitle)
				{
					_notifyOnFinishSubtitle = false;
					if (FinishedSubtitleHandler != null) FinishedSubtitleHandler(this, EventArgs.Empty);
				}
			}
		}

		public void StartResponses(Subtitle stateSubtitle, Response[] responses)
		{
			_dialogueUI.ShowResponses(stateSubtitle, responses);
		}
	}
}