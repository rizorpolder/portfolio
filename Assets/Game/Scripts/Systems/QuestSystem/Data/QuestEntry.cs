using System;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	[Serializable]
	public class QuestEntry
	{
		public QuestState QuestState = QuestState.Unassigned;
	}
}