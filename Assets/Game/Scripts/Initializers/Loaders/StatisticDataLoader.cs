using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.SaveSystem;
using Game.Scripts.Systems.Statistic;
using Game.Scripts.Systems.Statistic.Data;
using Zenject;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public class StatisticDataLoader : ALocalDataLoader<StatisticData>
	{
		public override StatisticData DefaultData => StatisticData.Default;

		protected override SaveDataType _loadDataType => SaveDataType.Statistics;

		public StatisticDataLoader(IDataCache<StatisticData> cache) : base(cache)
		{
		}
	}
}