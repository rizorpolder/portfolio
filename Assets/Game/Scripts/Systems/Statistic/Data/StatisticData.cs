using System.Collections.Generic;
using Game.Scripts.Systems.Statistic.Data.Collectors;

namespace Game.Scripts.Systems.Statistic.Data
{
	public class StatisticData
	{
		public Dictionary<StatisticActionType, StatisticActionData> Data =
			new Dictionary<StatisticActionType, StatisticActionData>();

		public Dictionary<StatisticActionType, StatisticActionData> SessionData =
			new Dictionary<StatisticActionType, StatisticActionData>();

		public Dictionary<StatisticActionType, CollectorActionData> Collectors =
			new Dictionary<StatisticActionType, CollectorActionData>();

		public static StatisticData Default => new StatisticData();
	}
}