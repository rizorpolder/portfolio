using System;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.Session.Data;
using Zenject;

namespace Game.Scripts.Systems.Session
{
	public class SessionSystem : ISessionCommand, ISessionData, IInitializable
	{
		public event Action<SessionData> SessionDataChanged;

		private SessionData _sessionData;
		public SessionData SessionData => _sessionData;

		private int _playtime;

		private DateTime _startSession;
		private TimeSpan SessionTime => DateTime.Now.Subtract(_startSession);

		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<SessionData> _sessionDataCache;

		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
		}

		private void OnCacheReadyHandler()
		{
			_sessionData = _sessionDataCache.GetData();
			
		}

		public void ReplaceSessionData(SessionData data)
		{
			_sessionData = data;
			SessionDataChanged?.Invoke(_sessionData);
		}
		
		

		public int GetPlayTime()
		{
			return _playtime + (int) SessionTime.TotalSeconds;
		}

		public void StartSession()
		{
			_startSession = DateTime.Now;
		}

		public void FinishSession()
		{
			_playtime = GetPlayTime();
		}
	}

	public interface ISessionData
	{
		public SessionData SessionData { get; }
	}
}

public interface ISessionCommand
{
	public void ReplaceSessionData(SessionData data);

	public int GetPlayTime();
	public void StartSession();
	public void FinishSession();
}