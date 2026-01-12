using Game.Scripts.Systems.Statistic.Data;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class StatisticDataCache : IDataCache<StatisticData>
	{
		private StatisticData _data;

		void IDataCache<StatisticData>.SetData(StatisticData data) => _data = data;
		public StatisticData GetData() => _data ?? StatisticData.Default;
	}
}