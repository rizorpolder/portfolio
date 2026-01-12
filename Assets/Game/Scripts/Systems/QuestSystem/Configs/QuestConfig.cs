using System;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.QuestSystem
{
	[Serializable]
	public class QuestConfig
	{
		public string QuestID;
		public string QuestNameToken;
		public string QuestDescriptionToken;
		public string QuestGoalToken;
		public string GroupName;
		public int EntriesCount;
		public CustomAction SuccessAction;
		public CustomAction FailureAction;
		public bool IsLastOfGroup;
		public bool IsEndOfModule;

	}
}