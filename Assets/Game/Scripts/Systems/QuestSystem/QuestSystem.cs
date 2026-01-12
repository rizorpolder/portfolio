using System;
using System.Collections.Generic;
using System.Linq;
using AudioManager.Runtime.Core.Manager;
using Game.Scripts.Data;
using Game.Scripts.Systems.Actions;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Character;
using Game.Scripts.Systems.Conditions;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.QuestSystem.Data;
using Game.Scripts.Systems.SaveSystem;
using Game.Scripts.Systems.Statistic;
using Game.Scripts.Systems.Statistic.Data;
using Plugins.AudioManager.Runtime.Core;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.QuestSystem
{
	public class QuestSystem : IQuestListener, IQuestCommand, IInitializable
	{
		public event Action<string> OnQuestStateChanged;
		public event Action<string> OnQuestEntryStateChanged;

		private QuestsDatabase _database;
		private readonly Dictionary<string, QuestEntity> _questEntities = new Dictionary<string, QuestEntity>();
		private readonly Dictionary<string, QuestGroupEntity> _moduleEntities = new Dictionary<string, QuestGroupEntity>();

		[Inject] QuestDB _questDB;

		[Inject] private IPlayerCommand _playerCommand;

		[Inject] private CharacterFactory _characterFactory;

		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<QuestSaveData> _questDataCache;

		[Inject] private CustomActionsFactory _customActionsFactory;
		[Inject] private ISaveSystemCommand _saveSystemCommand;
		[Inject] private IStatisticCommand _statisticCommand;

		[Inject] private ManagerAudio _managerAudio;

		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
		}

		private void OnCacheReadyHandler()
		{
			_database ??= _questDB.GetInitialDB();

			ApplySaveData();
			RefreshQuests();
		}

		private void ApplySaveData()
		{
			var currentSaveData = _questDataCache.GetData();

			foreach (var quest in currentSaveData.Quests.Select(dict => dict.Value))
			{
				_database.SetQuestState(quest.QuestID, quest.QuestState);
				for (var index = 0; index < quest.QuestEntriesState.Count; index++)
				{
					var questState = quest.QuestEntriesState[index];
					_database.SetQuestEntryState(quest.QuestID, index, questState);
				}
			}

			foreach (var module in currentSaveData.QuestGroups.Select(dict => dict.Value))
			{
				_database.SetQuestGroup(module.GroupID,
					new Resource() { Type = ResourceType.Points, Value = module.Points });
			}
		}

		private void RefreshQuests()
		{
			var activeQuests = _database.GetAllQuests(QuestState.Active);
			_questEntities.Clear();
			_moduleEntities.Clear();
			foreach (var quest in activeQuests)
			{
				if (quest.IsGroupQuest && _database.GetGroup(quest.GroupName, out var moduleData))
				{
					var moduleEntity = CreateModuleEntity(moduleData);
					_moduleEntities.TryAdd(quest.GroupName, moduleEntity);
				}

				var entity = CreateQuestEntity(quest);
				ChangeQuestState(entity, quest.QuestState);
			}
		}

		private QuestEntity CreateQuestEntity(string questID)
		{
			if (_questEntities.TryGetValue(questID, out var entity))
				return entity;

			var data = _database.GetCurrentQuestData(questID);
			entity = CreateQuestEntity(data);
			return entity;
		}

		private QuestEntity CreateQuestEntity(QuestData questData)
		{
			var questEntity = new QuestEntity(questData.QuestID);

			if (questData.SuccessAction.ActionType != ActionType.None)
			{
				questEntity.ReplaceSuccessAction(questData.SuccessAction);
			}

			if (questData.FailureAction.ActionType != ActionType.None)
			{
				questEntity.ReplaceFailureAction(questData.FailureAction);
			}

			if (questData.IsGroupQuest)
			{
				questEntity.SetQuestGroupName(questData.GroupName);
			}

			if (questData.IsLastOfGroup)
			{
				questEntity.SetLastGroupQuest();
			}

			if (questData.IsEndOfModule)
			{
				questEntity.SetEndOfModule();
			}

			_questEntities.Add(questData.QuestID, questEntity);
			return questEntity;
		}

		public void SetNextQuestState(string questID)
		{
			int entryCount = _database.GetQuestEntryCount(questID);
			var entity = CreateQuestEntity(questID);
			for (int i = 0; i < entryCount; ++i)
			{
				var entryState = _database.GetQuestEntryState(questID, i);
				if (entryState != QuestState.Success)
				{
					_database.SetQuestEntryState(questID, i, QuestState.Success);
					OnQuestStateChanged?.Invoke(questID);

					// если это последняя стадия, помечаем квест пройденным
					if (i == entryCount - 1)
					{
						ChangeQuestState(entity, QuestState.Success);
					}
					else
					{
						SaveQuestData();
					}

					return;
				}
			}

			ChangeQuestState(entity, QuestState.Success);
		}

		public void ChangeQuestState(string questID, QuestState questState)
		{
			var currentState = _database.GetQuestState(questID);
			{
				if (currentState != QuestState.Active && currentState.Equals(questState))
					return;
			}


			if (!_questEntities.TryGetValue(questID, out var entity))
			{
				entity = CreateQuestEntity(questID);
			}

			ChangeQuestState(entity, questState);
		}

		private void ChangeQuestState(QuestEntity entity, QuestState state)
		{
			HandleState(entity, state);
		}

		public QuestData GetQuestData(string questID)
		{
			return _database.GetCurrentQuestData(questID);
		}

		public QuestSaveData GetQuestSaveData()
		{
			return _database.GetSaveData();
		}

		public bool IsQuestCompleted(string questName)
		{
			var data = _database.GetCurrentQuestData(questName);
			return data.QuestState == QuestState.Success;
		}

		private void HandleState(QuestEntity entity, QuestState state)
		{
			string questName = entity.Name;

			switch (state)
			{
				case QuestState.Active:
					SetQuestState(questName, QuestState.Active);
					break;
				case QuestState.Success:
					OnSuccessQuest(questName);
					break;
				case QuestState.Failed:
					OnFailedQuest(questName);
					break;
			}
		}

		private void OnSuccessQuest(string questName)
		{
			if (!_questEntities.Remove(questName, out var questEntity))
				return;

			SetQuestState(questName, QuestState.Success);

			if (questEntity.HasSuccessAction)
			{
				ExecuteQuestAction(questEntity.SuccessAction);
			}

			if (questEntity.IsLastGroupQuest)
			{
				if (_database.GetGroup(questEntity.GroupName, out var groupData))
					_playerCommand.AddResource(groupData.CurrentReward);
			}
			
			if (questEntity.IsEndOfModule)
			{
				_statisticCommand.CreateAction(StatisticActionType.ModuleComplete,parameter1:questEntity.GroupName, isStatistic: true);
			}
			
			_statisticCommand.CreateAction(StatisticActionType.CompleteQuest,parameter1: questName, isStatistic: true);

			_managerAudio.PlayAudioClip(TAudio.click);
		}

		private void OnFailedQuest(string questName)
		{
			if (!_questEntities.Remove(questName, out var questEntity))
				return;
			SetQuestState(questName, QuestState.Failed);
			if (questEntity.HasFailureAction)
			{
				ExecuteQuestAction(questEntity.FailureAction);
			}

			_managerAudio.PlayAudioClip(TAudio.click);
		}

		private void ExecuteQuestAction(CustomAction questActionData)
		{
			var action = _customActionsFactory.Create(questActionData.ActionData);
			action.Execute();
		}

		private void SetQuestState(string questName, QuestState state)
		{
			_database.SetQuestState(questName, state);
			OnQuestStateChanged?.Invoke(questName);
			SaveQuestData();
		}

		public void GetQuestProgress(string questID, out int current, out int max)
		{
			current = 0;
			max = _database.GetQuestEntryCount(questID);
			for (int i = 0; i < max; ++i)
			{
				var entryState = _database.GetQuestEntryState(questID, i);
				if (entryState == QuestState.Success)
				{
					++current;
				}
			}

			// если нет стадий, то берем состояние самого квеста
			if (max == 0)
			{
				max = 1;
				var state = _database.GetQuestState(questID);
				current = state == QuestState.Grantable || state == QuestState.Success ? 1 : 0;
			}
		}

		public int[] GetQuestProgress(string questID)
		{
			var max = _database.GetQuestEntryCount(questID);
			var result = new int[max];

			for (int i = 0; i < max; i++)
			{
				var entryState = _database.GetQuestEntryState(questID, i);
				result[i] = entryState == QuestState.Success ? 1 : 0;
			}

			return result;
		}

		public void SetEntryState(string questID, int entryIndex, QuestState state)
		{
			int entryCount = _database.GetQuestEntryCount(questID);
			var entity = CreateQuestEntity(questID);

			if (entryIndex < 0 || entryIndex >= entryCount)
			{
#if UNITY_EDITOR
				Debug.Log($"{questID}: entryIndex out of range with index {entryIndex}");
#endif
				return;
			}

			_database.SetQuestEntryState(questID, entryIndex, QuestState.Success);
			OnQuestEntryStateChanged?.Invoke(questID);
			for (int i = 0; i < entryCount; ++i)
			{
				var entryState = _database.GetQuestEntryState(questID, i);
				if (entryState != QuestState.Success)
				{
					SaveQuestData();
					return;
				}
			}

			ChangeQuestState(entity, QuestState.Success);
		}

		public QuestState GetEntryState(string questID, int entryIndex)
		{
			return _database.GetQuestEntryState(questID, entryIndex);
		}

		public bool HaveActiveQuests()
		{
			return _questEntities.Count > 0;
		}

		public bool CheckQuestCondition(QuestCondition condition)
		{
			var questName = condition.QuestName;
			var questData = _database.GetCurrentQuestData(questName);
			bool isStateIsEquals = questData.QuestState.Equals(condition.QuestState);

			if (isStateIsEquals && condition.QuestEntryConditions.Count > 0)
			{
				return CheckQuestEntryConditions(questName, condition);
			}

			return isStateIsEquals;
		}

		private bool CheckQuestEntryConditions(string questName, QuestCondition condition)
		{
			foreach (var entryCondition in condition.QuestEntryConditions)
			{
				var state = _database.GetQuestEntryState(questName, entryCondition.QuestEntryID);
				if (!state.Equals(entryCondition.QuestState))
				{
					return false;
				}
			}

			return true;
		}

		public List<QuestEntity> GetActiveQuests()
		{
			return _questEntities.Values.ToList();
		}

		public QuestState GetQuestState(string questID)
		{
			return _database.GetQuestState(questID);
		}

		private void SaveQuestData()
		{
			_saveSystemCommand.Save(SaveDataType.QuestData, force: true);
		}

		#region Modules

		private QuestGroupEntity CreateModuleEntity(QuestGroupData questGroupData)
		{
			var moduleEntity = new QuestGroupEntity(questGroupData.ID);
			moduleEntity.ReplaceResources(questGroupData.CurrentReward);

			_moduleEntities.Add(questGroupData.ID, moduleEntity);
			return moduleEntity;
		}

		public void QuestResourceAction(QuestRewardData data)
		{
			if (data.Type == QuestRewardActionType.Decrease)
			{
				_database.ReduceQuestGroupReward(data.moduleID, data.Resource, data.Token);
			}
			else
			{
				_database.IncreaseQuestGroupReward(data.moduleID, data.Resource, data.Token);
			}
		}

		public bool GetModuleData(string moduleID, out QuestGroupData data) => _database.GetGroup(moduleID, out data);

		public int GetModulesScore() => _database.GetAllGroups().Sum(module => module.CurrentReward.Value);

		#endregion
	}
}