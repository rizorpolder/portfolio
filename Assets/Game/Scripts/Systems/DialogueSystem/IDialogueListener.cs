using System;

namespace Game.Scripts.Systems.DialogueSystem
{
	public interface IDialogueListener
	{
		public event Action<string> OnConversationStart;
		public event Action OnConversationFinished;
	}
}