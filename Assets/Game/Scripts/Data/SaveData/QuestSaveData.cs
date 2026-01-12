using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.QuestSystem;
using Game.Scripts.Systems.QuestSystem.Data;
using Newtonsoft.Json;

namespace Game.Scripts.Data
{
	[Serializable]
	public class QuestSaveData
	{
		public Dictionary<string, QuestSaveDataEntry> Quests = new Dictionary<string, QuestSaveDataEntry>();
		public Dictionary<string, QuestGroupSaveData> QuestGroups = new Dictionary<string, QuestGroupSaveData>();

		public static QuestSaveData Default => new QuestSaveData()
		{
			Quests = new Dictionary<string, QuestSaveDataEntry>(),
			QuestGroups = new Dictionary<string, QuestGroupSaveData>()
		};

		public static QuestSaveData CreateSaveDataFromDB(QuestsDatabase database)
		{
			var result = new QuestSaveData();

			foreach (var data in database.GetAllQuests())
			{
				result.Quests.Add(data.QuestID,
					new QuestSaveDataEntry(data.QuestID,
						data.QuestState,
						data.QuestEntries.Select(x => x.QuestState).ToList()));
			}

			foreach (var moduleData in database.GetAllGroups())
			{
				result.QuestGroups.Add(moduleData.ID, new QuestGroupSaveData(moduleData.ID, moduleData.CurrentReward.Value));
			}

			return result;
		}
	}

	[Serializable]
	public class QuestSaveDataEntry
	{
		public string QuestID;
		public QuestState QuestState;
		public List<QuestState> QuestEntriesState;

		public QuestSaveDataEntry(string questID, QuestState questState)
		{
			QuestID = questID;
			QuestState = questState;
			QuestEntriesState = new List<QuestState>();
		}

		[JsonConstructor]
		public QuestSaveDataEntry(string questID, QuestState questState, List<QuestState> questEntriesState) : this(
			questID,
			questState)
		{
			QuestEntriesState = questEntriesState;
		}
	}

	[Serializable]
	public class QuestGroupSaveData
	{
		public string GroupID;
		public int Points;

		public QuestGroupSaveData(string groupID, int points)
		{
			GroupID = groupID;
			Points = points;
		}
	}
}