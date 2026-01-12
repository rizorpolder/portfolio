using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.SaveSystem;
using Game.Scripts.Systems.Statistic.Data;
using Game.Scripts.Systems.Statistic.Data.Collectors;
using Zenject;

namespace Game.Scripts.Systems.Statistic
{
	public class StatisticSystem : IStatisticData, IStatisticCommand, IStatisticListener, IInitializable, IDisposable
	{
		private readonly List<CollectorActionParams> completedCollectorsParams = new List<CollectorActionParams>();

		public event Action<CollectorActionParams> CollectorCompleted;
		public event Action<StatisticActionEntity> ActionCreated;

		[Inject] private ISessionCommand _sessionCommand;
		[Inject] private ISaveSystemCommand _saveSystemCommand;

		private StatisticData _statisticData;
		private bool _needSave;

		public StatisticData Stats => _statisticData;

		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<StatisticData> _statisticDataCache;

		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
		}

		private void OnCacheReadyHandler()
		{
			_statisticData = _statisticDataCache.GetData();
		}

		public void StartService()
		{
			_sessionCommand.StartSession();
			CheckCollectors();
		}

		public void CheckCollectors()
		{
			var collectors = _statisticData.Collectors;
			foreach (var collectorByType in collectors)
			{
				foreach (var collector in collectorByType.Value.Params)
				{
					if (collector.IsCompleted())
					{
						completedCollectorsParams.Add(collector);
					}
				}
			}

			if (completedCollectorsParams.Count > 0)
			{
				foreach (var collectorParams in completedCollectorsParams)
					CollectorCompleted?.Invoke(collectorParams);
				completedCollectorsParams.Clear();
			}
		}

		public void CreateAction(StatisticActionType type,
			int count = 1,
			string parameter1 = "",
			string parameter2 = "",
			bool isStatistic = false)
		{
			var parameters = new StatisticActionParams(count, parameter1, parameter2);
			CreateAction(type, parameters, isStatistic);
		}

		public void CreateAction(StatisticActionType type, StatisticActionParams actionData, bool isStatistic = false)
		{
			ProcessAction(new StatisticActionEntity(type, actionData, isStatistic));
		}

		public void ProcessAction(StatisticActionEntity entity)
		{
			if (entity.IsStatistic)
			{
				ToStatistic(entity);
			}

			ToSessionStatistic(entity);
			CheckCollectors(entity);

			if (_needSave)
			{
				_needSave = false;
				_saveSystemCommand.Save(SaveDataType.Statistics);
			}

			ActionCreated?.Invoke(entity);
		}

		private void CheckCollectors(StatisticActionEntity entity)
		{
			var data = _statisticData.Collectors;
			if (!data.ContainsKey(entity.Type))
				return;

			foreach (var collector in data[entity.Type].Params)
			{
				if (!collector.HasParameters() || collector.Equals(entity.Params))
				{
					collector.Count += entity.Params.Count;
					_needSave = true;
				}
			}

			CheckCollectors();
		}

		private void ToStatistic(StatisticActionEntity entity)
		{
			var data = _statisticData.Data;
			if (!data.ContainsKey(entity.Type))
			{
				var actionData = new StatisticActionData();
				data.Add(entity.Type, actionData);
			}

			data[entity.Type].Add(entity.Params.Clone());
			_needSave = true;
		}

		private void ToSessionStatistic(StatisticActionEntity entity)
		{
			var sessionData = _statisticData.SessionData;
			if (!sessionData.ContainsKey(entity.Type))
			{
				var sessionActionData = new StatisticActionData();
				sessionData.Add(entity.Type, sessionActionData);
			}

			sessionData[entity.Type].Add(entity.Params.Clone());
			_needSave = true;
		}

		public int GetStatistic(StatisticActionType action, string parameter1 = "", string parameter2 = "")
		{
			if (_statisticData.Data.ContainsKey(action))
			{
				var data = _statisticData.Data[action];
				return data.Value(parameter1, parameter2);
			}

			return 0;
		}

		public int GetSessionStatistic(StatisticActionType action, string parameter1 = "", string parameter2 = "")
		{
			if (_statisticData.SessionData.ContainsKey(action))
			{
				var data = _statisticData.SessionData[action];
				return data.Value(parameter1, parameter2);
			}

			return 0;
		}

		public void AddCollector(AActivitiesTask task)
		{
			_statisticData.Collectors.TryAdd(task.ActionType, new CollectorActionData());
			AddCollectorActionParams(task,_statisticData.Collectors[task.ActionType]);
		}

		private void AddCollectorActionParams(AActivitiesTask task, CollectorActionData collectorData)
		{
			var collector = new CollectorActionParams(task.Identifier,
				task.ActionParams.Count,
				task.ActionParams.Parameter1,
				task.ActionParams.Parameter2);
			collectorData.Params.Add(collector);
		}

		public void ForceCollectorCompletion(string id, StatisticActionType type)
		{
			if (!_statisticData.Collectors.ContainsKey(type))
				return;

			var parameters = new List<CollectorActionParams>(_statisticData.Collectors[type].Params);
			foreach (var param in parameters)
				CollectorCompleted?.Invoke(param);
		}

		public void RemoveCollector(StatisticActionType actionType)
		{
			if (_statisticData.Collectors.ContainsKey(actionType))
			{
				_statisticData.Collectors.Remove(actionType);
			}
		}

		public void ClearCollectorParams(StatisticActionType actionType, string id)
		{
			if (_statisticData.Collectors.ContainsKey(actionType))
			{
				_statisticData.Collectors[actionType].Params.RemoveAll(x => x.ID.Equals(id));
			}
		}

		public bool ContainsCollector(StatisticActionType type)
		{
			return _statisticData.Collectors.ContainsKey(type);
		}

		public CollectorActionData GetCollector(AActivitiesTask taskData)
		{
			var collectors = _statisticData.Collectors;
			var pair = collectors.FirstOrDefault(kv =>
				kv.Key == taskData.ActionType && kv.Value.Params.Exists(p => p.ID == taskData.Identifier));
			if (pair.Key == taskData.ActionType)
				return pair.Value;

			return null;
		}

		public void Dispose()
		{
			_sessionCommand.FinishSession();
			_saveSystemCommand.SaveForce(SaveDataType.Statistics);
		}
	}
}