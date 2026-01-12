namespace Game.Scripts.Systems.Statistic.Data
{
	public class StatisticActionEntity
	{
		public StatisticActionParams Params;
		public StatisticActionType Type;
		public bool IsStatistic;

		public StatisticActionEntity(StatisticActionType type, StatisticActionParams parameters, bool isStatistic)
		{
			Type = type;
			Params = parameters;
			IsStatistic = isStatistic;
		}
	}
}