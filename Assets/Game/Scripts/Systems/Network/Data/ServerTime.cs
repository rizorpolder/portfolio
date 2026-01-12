using System;
using UnityEngine;

namespace Game.Scripts.Systems.Network.Data
{
	public class ServerTime : IGameDateTime
	{
		private DateTime _initTime;
		private float _secondsFromStartupAfterInitTime;

		public ServerTime(DateTime initTime)
		{
			_initTime = initTime;
			_secondsFromStartupAfterInitTime = Time.realtimeSinceStartup;
		}

		public ServerTime()
		{
			_initTime = DateTime.Now;
			_secondsFromStartupAfterInitTime = Time.realtimeSinceStartup;
		}

		private static bool UseLocalTime => PlayerPrefs.GetInt("TimeAntiCheat", 1) != 1;

		public DateTime Value => UseLocalTime
			? DateTime.UtcNow
			: _initTime.AddSeconds(Time.realtimeSinceStartup - _secondsFromStartupAfterInitTime);

		public DateTime UtcNow => Value;

		public DateTime Now
		{
			get
			{
				var diff = DateTime.Now - DateTime.UtcNow;
				return Value.Add(diff);
			}
		}

		public DateTime Today => Now.Date;
	}

	public interface IGameDateTime
	{
		DateTime Now { get; }
		DateTime UtcNow { get; }
	}
}