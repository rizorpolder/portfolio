using System;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.UI.Common;
using Game.Scripts.UI.WindowsSystem;
using Zenject;

namespace Game.Scripts.Systems.DialogueSystem
{
	public class DialogueSystem : IDialogueData, IDialogueListener, IDialogueCommand
	{
		public event Action<string> OnConversationStart;
		public event Action OnConversationFinished;

		private DialogueDatabase _dialogueDatabase;

		public DialogueDatabase DialogueDatabase => _dialogueDatabase;

		private bool _conversationActive = false;
		public bool IsConversationActive => _conversationActive;

		[Inject] private GlobalUI _globalUI;
		[Inject] private WindowsController _windowsController;

		[Inject]
		public DialogueSystem(DialogueDB initialDB)
		{
			_dialogueDatabase = initialDB.GetInitialDB();
			_dialogueDatabase.Initialize();
		}

		public void StartConversation(string title)
		{
			if (_windowsController.HasActiveOrInProcessWindow)
				return;

			_conversationActive = true;
			OnConversationStart?.Invoke(title);
			_globalUI.HUD.Hide("conversation");
		}

		public void EndConversation()
		{
			_conversationActive = false;
			_globalUI.HUD.Show("conversation");
			OnConversationFinished?.Invoke();
		}
	}
}