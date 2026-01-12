using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.DialogueSystem;
using Game.Scripts.UI.WindowsSystem;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	public class DialogueAction : BaseAction<DialogueActionData>
	{
		public override event Action<IActionExecutable> OnActionExecuted;

		[Inject] private IDialogueCommand _dialogueCommand;
		[Inject] private IDialogueListener _dialogueListener;
		[Inject] private IDialogueData _dialogueData;
		[Inject] private WindowsController _windowsController;

		public override void Execute()
		{
			if (_dialogueData.IsConversationActive)
				return;

			StartAwaiter().Forget();
		}

		private async UniTaskVoid StartAwaiter()
		{
			await UniTask.WaitUntil(() => !_dialogueData.IsConversationActive);
			await UniTask.WaitUntil(() => !_windowsController.HasActiveOrInProcessWindow);
			_dialogueListener.OnConversationFinished += OnConversationCompleteHandler;
			_dialogueCommand.StartConversation(_data.DialogueID);
		}

		private void OnConversationCompleteHandler()
		{
			_dialogueListener.OnConversationFinished -= OnConversationCompleteHandler;
			OnActionExecuted?.Invoke(this);
		}
	}
}