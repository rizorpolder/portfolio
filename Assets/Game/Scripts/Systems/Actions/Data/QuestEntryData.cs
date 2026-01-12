using System;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.Actions.Data
{
	[Serializable]
	public class QuestEntryData : IActionData
	{
		public string QuestID;
		public QuestState NewEntryState;
		public int EntryIndex;
	}
}