using System;
using System.Collections.Generic;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.DialogueSystem
{
	[Serializable]
	public class DialogueEntryConfig
	{
		public int ID;
		public string Name;

		public int SpeakerID;
		public int ListenerID;
		public bool IsMentorText;
		public string DialogueTextToken;
		public List<DialogueLink> NextEntriesLinks = new List<DialogueLink>();
		public List<CustomAction> EndEvents = new List<CustomAction>();
		public bool AutoTransition;
	}

	[Serializable]
	public class DialogueLink
	{
		public int TargetEntryID;
		public string ButtonText;

	}

}