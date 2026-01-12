using System;
using System.Collections.Generic;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.Conditions
{
	[Serializable]
	public class Condition
	{
		public QuestCondition[] QuestConditions;
	}

	[Serializable]
	public class QuestCondition
	{
		public string QuestName;
		public QuestState QuestState;
		public List<QuestEntryCondition> QuestEntryConditions = new List<QuestEntryCondition>();
	}

	[Serializable]
	public class QuestEntryCondition
	{
		public int QuestEntryID;
		public QuestState QuestState;
	}
}