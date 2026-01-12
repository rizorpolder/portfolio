using Game.Scripts.Systems.Actions;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.UI.DialogueSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class DialogueController : MonoBehaviour
	{
		[SerializeField] private DialogueUI _view;

		[Inject] private IDialogueListener _dialogueListener;
		[Inject] private IDialogueData _dialogueData;
		[Inject] private IDialogueCommand _dialogueCommand;
		[Inject] private CustomActionsFactory _customActionsFactory;

		private Conversation _currentConversation;
		private ConversationController _conversationController;

		private void Start()
		{
			_dialogueListener.OnConversationStart += OnConversationStarted;
		}

		private void OnConversationStarted(string title)
		{
			_currentConversation = _dialogueData.DialogueDatabase.GetConversation(title);
			_conversationController = new ConversationController();
			_conversationController.Initialize(_dialogueData.DialogueDatabase, _view, _currentConversation);
			_conversationController.AddCallback(ResetState);
			_conversationController.AddFinishSubtitleCallback(ExecuteActions);
			_conversationController.StartConversation();
		}

		private void ExecuteActions(CustomAction[] events)
		{
			foreach (var e in events)
			{
				if (e.ActionType == ActionType.None)
					continue;

				var ev = _customActionsFactory.Create(e.ActionData);
				ev.Execute();
			}
		}

		private void ResetState()
		{
			_currentConversation = null;
			_conversationController = null;
			_dialogueCommand.EndConversation();
		}

		private void OnDestroy()
		{
			_dialogueListener.OnConversationStart -= OnConversationStarted;
		}
	}
}