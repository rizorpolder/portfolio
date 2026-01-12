using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.DialogueSystem
{
	public interface IDialogueData
	{
		public bool IsConversationActive { get; }
		public DialogueDatabase DialogueDatabase { get; }
	}
}