using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Systems.Statistic.Data
{
	[Serializable]
	public class StatisticActionData
	{
		public List<StatisticActionParams> Params = new List<StatisticActionParams>();

		public int Value(string parameter1 = null, string parameter2 = null)
		{
			if (string.IsNullOrEmpty(parameter1) && string.IsNullOrEmpty(parameter2))
			{
				return Params.Sum(x => x.Count);
			}
			else if (string.IsNullOrEmpty(parameter2))
			{
				return Params.Where(x => x.Parameter1.Equals(parameter1)).Sum(x => x.Count);
			}
			else if (string.IsNullOrEmpty(parameter1))
			{
				return Params.Where(x => x.Parameter2.Equals(parameter2)).Sum(x => x.Count);
			}
			else
			{
				return Params.Where(x => x.Parameter1.Equals(parameter1)
				                         && x.Parameter2.Equals(parameter2)).Sum(x => x.Count);
			}
		}

		public void Add(StatisticActionParams actionParams)
		{
			var data = Params.Find(x => x.Equals(actionParams));
			if (data == null)
			{
				Params.Add(actionParams);
			}
			else
			{
				data.Count += actionParams.Count;
			}
		}

		public void ReplaceFirst(StatisticActionParams actionParams)
		{
			if (Params.Count == 0)
			{
				Params.Add(actionParams);
				return;
			}

			Params[0] = actionParams;
		}
	}
}