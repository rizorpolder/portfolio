using AudioManager.Runtime.Extensions;

namespace Game.Scripts.Systems.Statistic.Data
{
	[System.Serializable]
	public class StatisticActionParams
	{
		public int Count;
		public string Parameter1;
		public string Parameter2;

		public StatisticActionParams Clone()
		{
			return new StatisticActionParams(Count, Parameter1, Parameter2);
		}

		public StatisticActionParams()
		{
		}

		public StatisticActionParams(string parameter1)
		{
			Count = 1;
			Parameter1 = parameter1;
		}

		public StatisticActionParams(int count)
		{
			Count = count;
		}

		public StatisticActionParams(int count, string param1, string param2)
		{
			Count = count;
			Parameter1 = param1;
			Parameter2 = param2;
		}

		public StatisticActionParams(int count, string param1)
		{
			Count = count;
			Parameter1 = param1;
		}

		public bool Equals(StatisticActionParams other)
		{
			if (this == other)
				return true;

			if (other == null)
				return false;

			if (Parameter1 != other.Parameter1)
				return false;

			if (!this.Parameter2.IsNullOrEmpty() && Parameter2 != other.Parameter2)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			return Parameter1.GetHashCode() + Parameter2.GetHashCode();
		}
	}
}