using System;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.DialogueSystem.Events;
using Game.Scripts.UI.DialogueSystem;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class ConversationController
	{
		private Action<CustomAction[]> OnFinishSubtitle;
		private Action OnConversationComplete;
		private ConversationView _view;
		private ConversationState _state;

		private ConversationModel _model;

		private bool _isActive = false;

		public void Initialize(DialogueDatabase database, DialogueUI view, Conversation conversation)
		{
			_isActive = true;
			_view = new ConversationView();
			_model = new ConversationModel(database, conversation.Title);
			_view.Initialize(view);
			_view.FinishedSubtitleHandler += OnFinishedSubtitle;
			_view.SelectedResponseHandler += OnSelectedResponse;
		}

		public void AddCallback(Action callback)
		{
			OnConversationComplete = callback;
		}

		public void AddFinishSubtitleCallback(Action<CustomAction[]> callback)
		{
			OnFinishSubtitle = callback;
		}

		public void StartConversation()
		{
			GotoState(_model.FirstState);
		}

		private void OnFinishedSubtitle(object sender, EventArgs e)
		{
			OnFinishSubtitle?.Invoke(_state.Events);

			if (_state.HasNpcResponse)
			{
				GotoState(_model.GetState(_state.FirstNpcResponse.DestinationEntry));
			}
			else if (_state.HasPcResponse)
			{
				GotoState(_model.GetState(_state.FirstPcResponse.DestinationEntry));
			}
			else
			{
				Close();
			}
		}

		private void OnSelectedResponse(object sender, SelectedResponseEventArgs e)
		{
			GotoState(_model.GetState(e.DestinationEntry));
		}

		private void GotoState(ConversationState state)
		{
			_state = state;
			if (state != null)
			{
				_view.StartSubtitle(state.Subtitle);
				var destResp = state.HasNpcResponse ? state.NpcResponses :
					state.HasPcResponse ? state.PcResponses : null;
				if (destResp?.Length > 1)
				{
					_view.StartResponses(state.Subtitle, destResp);
				}
			}
			else
			{
				Close();
			}
		}

		private void Close()
		{
			if (!_isActive)
				return;

			_isActive = false;
			_view.FinishedSubtitleHandler -= OnFinishedSubtitle;
			_view.SelectedResponseHandler -= OnSelectedResponse;
			_view.Close();
			OnConversationComplete?.Invoke();
		}
	}
}