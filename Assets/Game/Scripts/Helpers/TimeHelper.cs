using System;

namespace Game.Scripts.Helpers
{
	public static class TimeHelper
	{
		private static string shortDayText;
		private static string shortHourText;
		private static string shortMinuteText;
		private static string shortSecondText;

		public static readonly DateTime DataConst = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		public static readonly DateTime DataConstNoUTC = new DateTime(1970, 1, 1, 0, 0, 0);

		// todo вынести в конфиг или оставить так после обсуждения с гейм-дизайнерами, нужно ли это!?
		public static readonly TimeSpan LiveCooldownTime = TimeSpan.FromMinutes(20);

		public static string ShortDay()
		{
			if (string.IsNullOrEmpty(shortDayText))
				shortDayText = "d";

			return shortDayText;
		}

		public static string ShortHour()
		{
			if (string.IsNullOrEmpty(shortHourText))
				shortHourText = "h";

			return shortHourText;
		}

		public static string ShortMinute()
		{
			if (string.IsNullOrEmpty(shortMinuteText))
				shortMinuteText = "m";

			return shortMinuteText;
		}

		public static string ShortSecond()
		{
			if (string.IsNullOrEmpty(shortSecondText))
				shortSecondText = "s";

			return shortSecondText;
		}

		public static string FormattedDate(System.TimeSpan time)
		{
			if (time.TotalHours > 24)
			{
				return $"{time.Days}{ShortDay()} {time.Hours}{ShortHour()}";
			}
			else if (time.TotalMinutes >= 60)
			{
				return $"{time.Hours}{ShortHour()} {time.Minutes}{ShortMinute()}";
			}

			return $"{time.Minutes:D2}{ShortMinute()} {time.Seconds:D2}{ShortSecond()}";
		}

		public static string FormattedDate(long ticks)
		{
			return FormattedDate(new TimeSpan(ticks - DateTime.Now.Ticks)); //TODO Server Time
		}

		public static string FormattedHours(double hours)
		{
			if (hours >= 1d)
			{
				return $"{hours}{ShortHour()}";
			}
			else
			{
				int minutes = (int) Math.Round(hours * 60);
				return $"{(minutes)}{ShortMinute()}";
			}
		}

		public static TimeSpan ToTimespanFromSeconds(this float time)
		{
			return TimeSpan.FromSeconds(time);
		}

		public static DateTime FromUnixTimeMs(long timestampMs)
		{
			return DataConst.AddMilliseconds(timestampMs);
		}

		public static long ToUnixTimeMs(DateTime timestamp)
		{
			return (long) timestamp.Subtract(DataConst).TotalMilliseconds;
		}
	}
}