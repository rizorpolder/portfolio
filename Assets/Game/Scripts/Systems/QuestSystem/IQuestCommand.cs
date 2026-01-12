using System.Collections.Generic;
using Game.Scripts.Data;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Conditions;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.QuestSystem
{
	public interface IQuestCommand
	{
		public QuestSaveData GetQuestSaveData();
		
		public bool IsQuestCompleted(string questName);
		public void SetNextQuestState(string questID);
		public void SetEntryState(string questID, int entryIndex, QuestState state);
		public QuestState GetEntryState(string questID, int entryIndex);
		public QuestState GetQuestState(string questID);
		public void ChangeQuestState(string questID, QuestState questState);
		
		public QuestData GetQuestData(string questID);
		public List<QuestEntity> GetActiveQuests();
		public bool HaveActiveQuests();
		public bool CheckQuestCondition(QuestCondition condition);

		public void GetQuestProgress(string questID, out int current, out int max);
		public int[] GetQuestProgress(string questID);
		public void QuestResourceAction(QuestRewardData data);

		public bool GetModuleData(string moduleID, out QuestGroupData data);
		public int GetModulesScore();
	}
}