using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Helpers;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.PlayerController.Data;
using UnityEngine;
using Zenject;

namespace CooldownSystem
{
	public class CooldownManager : IInitializable
	{
		public event Action<Timer> TimerCompleted;

		private readonly CooldownService _cooldownService;

		private readonly Dictionary<string, Timer> _timers = new Dictionary<string, Timer>();

		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<PlayerData> _playerDataCache;
		private PlayerData _playerData;
		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
		}

		private void OnCacheReadyHandler()
		{
			_playerData = _playerDataCache.GetData();
		}
		
		
		public CooldownManager()
		{
			_cooldownService = new CooldownService();
			_cooldownService.CooldownCompleted += CooldownCompletionHandler;
		}

		public void InitTimers()
		{
			foreach (var timer in _playerData.Timers)
			{
				SetTimer(timer);
			}
		}

		public void SaveTimers(PlayerData playerData) 
		{
			playerData.Timers = new List<TimerData>();

			foreach (var timer in _timers)
			{
				playerData.Timers.Add(new TimerData(timer.Value.Cooldown));
			}
		}

		public Timer SetTimer(CooldownTypes type, DateTime time)
		{
			var id = GetCooldownTypeName(type);
			return SetTimer(id, time);
		}

		public Timer SetTimer(string id, DateTime time)
		{
			var timer = GetTimer(id);
			timer.Cooldown.CompletionDate = time;
			TryRegisterTimer(timer);
			return timer;
		}

		public void SetTimer(TimerData timerData)
		{
			var id = timerData.Id;
			var time = DateTime.FromBinary(timerData.Value);

			var timer = GetTimer(id);
			timer.Cooldown.Duration = timerData.Duration;
			timer.Cooldown.StartDate = DateTime.FromBinary(timerData.StartTime);
			if (timerData.IsFreeze)
				timer.Cooldown.Freeze(TimeSpan.FromMilliseconds(timerData.FreezeTimestamp));

			timer.Cooldown.CompletionDate = time;

			TryRegisterTimer(timer);
		}

		private string GetCooldownTypeName(CooldownTypes type)
		{
			return Enum.GetName(typeof(CooldownTypes), type);
		}

		private void TryRegisterTimer(Timer timer)
		{
			if (timer.Cooldown.IsComplete)
				return;

			if (_cooldownService.IsAlreadyRegistered(timer.Cooldown.Id))
				return;

			_cooldownService.RegisterTimer(timer);
		}

		public Timer AddCooldown(string id, double durationHours)
		{
			DateTime completionTime =DateTime.Now + TimeSpan.FromHours(durationHours); //TODO Server time
			return SetTimer(id, completionTime);
		}

		public Timer GetTimer(CooldownTypes type)
		{
			var id = GetCooldownTypeName(type);
			return GetTimer(id);
		}

		public Timer GetTimer(string id, bool createIfNotExist = true)
		{
			if (!HasTimer(id))
				return createIfNotExist ? CreateTimer(id) : null;

			return _timers[id];
		}

		public bool HasTimer(string id)
		{
			return _timers.ContainsKey(id);
		}

		public bool HasTimer(CooldownTypes type)
		{
			return HasTimer(GetCooldownTypeName(type));
		}

		private Timer CreateTimer(string id)
		{
			var timer = new Timer();
			timer.SetCooldown(new Cooldown(id, TimeHelper.DataConst));
			_timers.Add(id, timer);

			return timer;
		}

		public List<Timer> GetTimers()
		{
			var timers = _timers.Values.ToList();
			return timers;
		}

		public void RemoveTimer(string id)
		{
			RemoveTimerByid(id);
		}

		public void RemoveTimer(CooldownTypes type)
		{
			var id = GetCooldownTypeName(type);
			RemoveTimerByid(id);
		}

		private void RemoveTimerByid(string id)
		{
			_cooldownService.RemoveTimer(id);
			_timers.Remove(id);
		}

		public void CooldownCompletionHandler(Cooldown cooldown)
		{
			if (!_timers.ContainsKey(cooldown.Id))
			{
				Debug.LogError($"Completed cooldown {cooldown.Id} but timer is missing!");
				return;
			}

			var timer = GetTimer(cooldown.Id);
			TimerCompleted?.Invoke(timer);
		}

		public void Wipe()
		{
			_cooldownService.Wipe();
			_timers.Clear();
		}
	}
}