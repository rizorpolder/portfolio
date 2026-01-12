using System.Collections.Generic;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class DialogueEntry : Asset
	{
		private int _actorID;
		private int _conversantID;

		private int _conversationID;

		private string _title;
		private string _subtitleText;
		private bool _isMentorText = false;
		private bool _autoTransition = false;
		public int ActorID => _actorID;
		public int ConversantID => _conversantID;
		public int ConversationID => _conversationID;
		public string Title => _title;
		
		public bool IsMentorText => _isMentorText;
		public bool AutoTransition => _autoTransition;
		public List<DialogueLink> OutgoingLinks = new List<DialogueLink>();

		public List<CustomAction> EndEvents = new List<CustomAction>();

		public DialogueEntry(DialogueEntry sourceAsset) : base(sourceAsset)
		{
			_actorID = sourceAsset._actorID;
			_conversantID = sourceAsset._conversantID;
			_title = sourceAsset._title;
			_subtitleText = sourceAsset._subtitleText;
			_conversationID = sourceAsset._conversationID;
			OutgoingLinks = sourceAsset.OutgoingLinks;
			EndEvents = sourceAsset.EndEvents;
			_isMentorText =  sourceAsset.IsMentorText;
			_autoTransition = sourceAsset.AutoTransition;
		}

		public DialogueEntry(DialogueEntryConfig sourceConfig) : base(sourceConfig.ID, sourceConfig.Name)
		{
			_actorID = sourceConfig.SpeakerID;
			_conversantID = sourceConfig.ListenerID;
			_title = sourceConfig.DialogueTextToken;
			_conversationID = sourceConfig.ID;
			OutgoingLinks = sourceConfig.NextEntriesLinks;
			EndEvents = sourceConfig.EndEvents;
			_isMentorText = sourceConfig.IsMentorText;
			_autoTransition = sourceConfig.AutoTransition;
		}
	}
}