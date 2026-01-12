using Game.Scripts.Systems.Session.Data;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class SessionDataCache : IDataCache<SessionData>
	{
		private SessionData _data;

		void IDataCache<SessionData>.SetData(SessionData data) => _data = data;
		public SessionData GetData() => _data ?? SessionData.Default;
	}
}