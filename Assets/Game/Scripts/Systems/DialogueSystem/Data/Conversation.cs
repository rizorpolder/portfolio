using System.Collections.Generic;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class Conversation : Asset
	{
		private string _conversationTitle;
		public string Title => _conversationTitle;

		private List<DialogueEntry> _dialogueEntries = new List<DialogueEntry>();


		public Conversation(Conversation sourceAsset) : base(sourceAsset)
		{
			_conversationTitle = sourceAsset._conversationTitle;
			_dialogueEntries = CopyDialogueEntries(sourceAsset._dialogueEntries);
		}

		public Conversation(ConversationConfig sourceConfig) : base(0, "")
		{
			_conversationTitle = sourceConfig.ConversationID;
			foreach (var entry in sourceConfig.DialogueEntries)
			{
				_dialogueEntries.Add(new DialogueEntry(entry));
			}
		}

		private List<DialogueEntry> CopyDialogueEntries(List<DialogueEntry> sourceEntries)
		{
			var entries = new List<DialogueEntry>();
			foreach (var sourceEntry in sourceEntries)
			{
				entries.Add(new DialogueEntry(sourceEntry));
			}

			return entries;
		}

		public DialogueEntry GetFirstDialogueEntry()
		{
			return _dialogueEntries.Find(x => x.ID.Equals(0));
		}

		public DialogueEntry GetDialogueEntry(int dialogueEntryID)
		{
			return _dialogueEntries.Find(x => x.ID.Equals(dialogueEntryID));
		}
	}


}