using System;
using System.Collections.Generic;

namespace Game.Scripts.Systems.Statistic.Data.Collectors
{
	[Serializable]
	public class CollectorActionData
	{
		public List<CollectorActionParams> Params = new List<CollectorActionParams>();
	}
}