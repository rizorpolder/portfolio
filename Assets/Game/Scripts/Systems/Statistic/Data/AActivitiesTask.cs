using System;

namespace Game.Scripts.Systems.Statistic.Data
{
	[Serializable]
	public abstract class AActivitiesTask
	{
		public abstract string Identifier { get; }
		public abstract StatisticActionType ActionType { get; }
		public abstract StatisticActionParams ActionParams { get; }
	}
}
