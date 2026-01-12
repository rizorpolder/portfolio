
namespace Game.Scripts.Systems.DialogueSystem
{
	public interface IDialogueCommand
	{
		public void StartConversation(string id);
		public void EndConversation();
	}
}