using System;

namespace Game.Scripts.Systems.Session.Data
{
	public class SessionData
	{
		public int SessionNumber;
		public int SessionTodayNumber;
		public long LastSessionDate;
		public long CurrentSessionDate;
		public long FirstLaunchDate;

		public long LastPurchaseDate;

		public static SessionData Default = new SessionData
		{
			SessionNumber = 0,
			LastSessionDate = 0,
			CurrentSessionDate = 0,
			FirstLaunchDate = DateTime.Now.Ticks, //TODO FromServerTime (NetworkSystemData)
		};

		public SessionData TrackSession()
		{
			SessionNumber++;
			LastSessionDate = CurrentSessionDate == 0 ? DateTime.Now.Ticks : CurrentSessionDate; //TODO FromServerTime (NetworkSystemData)
			CurrentSessionDate = DateTime.Now.Ticks; //TODO FromServerTime (NetworkSystemData)

			if (FirstLaunchDate == 0)
				FirstLaunchDate = DateTime.Now.Ticks; //TODO FromServerTime (NetworkSystemData)

			var lastDate = new DateTime(LastSessionDate).Date;
			if (lastDate == DateTime.Now.Date) //TODO FromServerTime (NetworkSystemData)
			{
				++SessionTodayNumber;
			}
			else
			{
				SessionTodayNumber = 1;
			}

			return this;
		}

		public float DaysFromLastSession()
		{
			DateTime last = new DateTime(LastSessionDate);
			TimeSpan delta = DateTime.Now - last; //TODO FromServerTime (NetworkSystemData)
			return (float) delta.TotalDays;
		}

		public float HoursFromLastSession()
		{
			var lastSessionData = new DateTime(LastSessionDate);
			var delta = DateTime.Now - lastSessionData; //TODO FromServerTime (NetworkSystemData)
			return (float) delta.TotalHours;
		}

		public float DaysFromFirstLaunch()
		{
			if (FirstLaunchDate == 0)
				return -1;

			DateTime launchDate = new DateTime(FirstLaunchDate);
			TimeSpan delta = DateTime.Now - launchDate; //TODO FromServerTime (NetworkSystemData)
			return (float) delta.TotalDays;
		}

		public double MinutesFromFirstLaunch()
		{
			if (FirstLaunchDate == 0)
				return -1;

			var firstLaunchDate = new DateTime(FirstLaunchDate);
			var delta = DateTime.Now - firstLaunchDate; //TODO FromServerTime (NetworkSystemData)
			return delta.TotalMinutes;
		}

		public float DaysFromLastPurchase()
		{
			if (LastPurchaseDate == 0)
				return -1;

			DateTime last = new DateTime(LastPurchaseDate);
			TimeSpan delta = DateTime.Now - last; //TODO FromServerTime (NetworkSystemData)
			return (float) delta.TotalDays;
		}

		public bool HasAnyPurchase()
		{
			return LastPurchaseDate != 0;
		}

		public SessionData UpdateLastPurchaseDate()
		{
			LastPurchaseDate = DateTime.Now.Ticks; //TODO FromServerTime (NetworkSystemData)
			return this;
		}
	}
}