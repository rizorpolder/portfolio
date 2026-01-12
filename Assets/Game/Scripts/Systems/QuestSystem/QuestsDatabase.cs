using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Data;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.QuestSystem
{
	public class QuestsDatabase
	{
		private Dictionary<string, QuestData> _questsCache = new();
		private Dictionary<string, QuestGroupData> _groupsCache = new();

		public void AddQuest(QuestData quest)
		{
			_questsCache.Add(quest.QuestID, quest);
		}

		public void AddGroup(QuestGroupData questGroup)
		{
			_groupsCache.Add(questGroup.ID, questGroup);
		}

		public List<QuestData> GetAllQuests(QuestState state)
		{
			return _questsCache.Values.Where(x => x.QuestState.Equals(state)).ToList();
		}

		public  bool GetGroup(string groupId, out QuestGroupData questGroupData)
		{
			return _groupsCache.TryGetValue(groupId, out questGroupData);
		}

		public void SetQuestState(string questName, QuestState state)
		{
			if (!_questsCache.TryGetValue(questName, out var quest))
				return;

			var prevState = quest.QuestState;
			quest.QuestState = state;

			if (prevState.Equals(state))
				return;

			foreach (var entry in quest.QuestEntries)
			{
				entry.QuestState = state == QuestState.Active ? QuestState.Unassigned : state;
			}
		}

		public QuestState GetQuestState(string questName)
		{
			if (!_questsCache.TryGetValue(questName, out var quest))
			{
				return QuestState.Unassigned;
			}

			return quest.QuestState;
		}

		public void SetQuestEntryState(string questName, int i, QuestState state)
		{
			if (!_questsCache.TryGetValue(questName, out var quest))
				return;
			quest.QuestEntries[i].QuestState = state;
		}

		public int GetQuestEntryCount(string questName)
		{
			if (!_questsCache.TryGetValue(questName, out var quest))
				return -1;
			return quest.QuestEntries.Count;
		}

		public QuestState GetQuestEntryState(string questName, int i)
		{
			if (!_questsCache.TryGetValue(questName, out var quest))
				return QuestState.Unassigned;
			return quest.QuestEntries[i].QuestState;
		}

		public QuestData GetCurrentQuestData(string questID)
		{
			if (!_questsCache.TryGetValue(questID, out var quest))
				return null;
			return new QuestData(quest);
		}

		public IEnumerable<QuestData> GetAllQuests() => _questsCache.Values;
		public IEnumerable<QuestGroupData> GetAllGroups() => _groupsCache.Values;
		

		public QuestSaveData GetSaveData()
		{
			return QuestSaveData.CreateSaveDataFromDB(this);
		}

		public void ReduceQuestGroupReward(string groupID, Resource dataResource, string reasonToken)
		{
			if (!_groupsCache.TryGetValue(groupID, out var group))
				return;
			group.ReduceQuestGroupReward(reasonToken, dataResource);
		}

		public void IncreaseQuestGroupReward(string groupID, Resource dataResource, string dataToken)
		{
			if (!_groupsCache.TryGetValue(groupID, out var group))
				return;
			group.AddQuestGroupReward(groupID, dataResource);
		}

		public void SetQuestGroup(string groupID, Resource dataResource)
		{
			if (!_groupsCache.TryGetValue(groupID, out var group))
				return;
			
			group.CurrentReward =  dataResource;
		}
	}
}