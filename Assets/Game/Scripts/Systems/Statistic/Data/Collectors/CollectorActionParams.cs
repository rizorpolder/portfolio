namespace Game.Scripts.Systems.Statistic.Data.Collectors
{
	public class CollectorActionParams : StatisticActionParams
	{
		public string ID;
		public int TargetValue;

		public CollectorActionParams()
		{
		}

		public CollectorActionParams(string id, int target, string param1, string param2)
		{
			ID = id;
			TargetValue = target;
			Parameter1 = param1;
			Parameter2 = param2;
		}

		public bool HasParameters()
		{
			return !string.IsNullOrEmpty(Parameter1) || !string.IsNullOrEmpty(Parameter2);
		}

		public bool IsCompleted()
		{
			return Count >= TargetValue;
		}

		public string GetProgress()
		{
			int current = Count > TargetValue ? TargetValue : Count;
			return $"{current}/{TargetValue}";
		}

		public void ResetProgress()
		{
			Count = 0;
		}
	}
}