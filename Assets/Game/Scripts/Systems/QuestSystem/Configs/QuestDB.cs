using System.Collections.Generic;
using Game.Scripts.Systems.QuestSystem.Data;
using UnityEngine;

namespace Game.Scripts.Systems.QuestSystem
{
	[CreateAssetMenu(fileName = "QuestDB", menuName = "Project/Quests/QuestDB")]
	public class QuestDB : ScriptableObject
	{
		public List<QuestConfig> Quests;
		public List<QuestGroupConfig> Groups;

		public QuestsDatabase GetInitialDB()
		{
			var initDB = new QuestsDatabase();
			foreach (var quest in Quests)
			{
				initDB.AddQuest(new QuestData(quest));
			}

			foreach (var module in Groups)
			{
				initDB.AddGroup(new QuestGroupData(module));
			}

			return initDB;
		}
	}
}