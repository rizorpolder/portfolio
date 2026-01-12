using Game.Scripts.Systems.Statistic.Data;
using Game.Scripts.Systems.Statistic.Data.Collectors;

namespace Game.Scripts.Systems.Statistic
{
	public interface IStatisticCommand
	{
		public void StartService();
		public int GetSessionStatistic(StatisticActionType action, string parameter1 = "", string parameter2 = "");
		public int GetStatistic(StatisticActionType action, string parameter1 = "", string parameter2 = "");

		public void AddCollector(AActivitiesTask task);

		public CollectorActionData GetCollector(AActivitiesTask taskData);
		public void CheckCollectors();
		public bool ContainsCollector(StatisticActionType type);
		public void RemoveCollector(StatisticActionType actionType);

		public void ClearCollectorParams(StatisticActionType actionType, string id);
		public void ForceCollectorCompletion(string id, StatisticActionType type);

		public void CreateAction(StatisticActionType type,
			int count = 1,
			string parameter1 = "",
			string parameter2 = "",
			bool isStatistic = false);
	}
}