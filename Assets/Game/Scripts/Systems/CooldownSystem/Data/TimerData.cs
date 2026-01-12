namespace CooldownSystem
{
	public class TimerData
	{
		public string Id;
		public long Value;
		public bool IsFreeze;
		public double FreezeTimestamp;
		public double Duration;
		public long StartTime;

		public TimerData(Cooldown cooldown)
		{
			Id = cooldown.Id;
			Value = cooldown.CompletionDate.ToBinary();
			IsFreeze = cooldown.IsFreeze;
			FreezeTimestamp = cooldown.FreezeTime.TotalMilliseconds;
			Duration = cooldown.Duration;
			StartTime = cooldown.StartDate.ToBinary();
		}
	}
}