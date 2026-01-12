using System;
using System.Collections.Generic;
using AudioManager.Runtime.Extensions;
using Game.Scripts.Extensions;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	[Serializable]
	public class QuestData
	{
		public string QuestID;
		public string QuestNameToken;
		public string QuestGoalToken;
		public string GroupName;
		public string QuestDescritpionToken;
		public string QuestSuccessToken;
		public QuestState QuestState = QuestState.Unassigned;
		public List<QuestEntry> QuestEntries = new List<QuestEntry>();

		public CustomAction SuccessAction;
		public CustomAction FailureAction;

		public bool IsGroupQuest => !GroupName.IsNullOrEmpty();
		public bool IsEndOfModule = false;
		public bool IsLastOfGroup = false;
		
		public QuestData(QuestData entryData)
		{
			QuestID = entryData.QuestID;
			QuestNameToken = entryData.QuestNameToken;
			QuestGoalToken = entryData.QuestGoalToken;
			QuestDescritpionToken = entryData.QuestDescritpionToken;
			QuestSuccessToken = entryData.QuestSuccessToken;
			QuestState = entryData.QuestState;
			QuestEntries = entryData.QuestEntries;
			GroupName =  entryData.GroupName;
			SuccessAction = entryData.SuccessAction;
			FailureAction = entryData.FailureAction;
			IsEndOfModule = entryData.IsEndOfModule;
			IsLastOfGroup =  entryData.IsLastOfGroup;
		}

		public QuestData(QuestConfig config)
		{
			QuestID = config.QuestID;
			QuestNameToken = config.QuestNameToken;
			QuestDescritpionToken = config.QuestDescriptionToken;
			QuestGoalToken = config.QuestGoalToken;
			for (int i = 0; i < config.EntriesCount; i++)
			{
				QuestEntries.Add(new QuestEntry());
			}

			GroupName = config.GroupName;
			SuccessAction = config.SuccessAction;
			FailureAction = config.FailureAction;
			IsEndOfModule = config.IsEndOfModule;
			IsLastOfGroup = config.IsLastOfGroup;
		}
	}
}