using System;
using Game.Scripts.Systems.Statistic.Data.Collectors;

namespace Game.Scripts.Systems.Statistic
{
	public interface IStatisticListener
	{
		public event Action<CollectorActionParams> CollectorCompleted;
	}
}