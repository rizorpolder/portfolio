using System;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.Actions.Data
{
	[Serializable]
	public class QuestActionData : IActionData
	{
		public string QuestID;
		public QuestState QuestState = QuestState.Unassigned;
	}
}