using Game.Scripts.Systems.SaveSystem;
using Game.Scripts.Systems.Session.Data;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public class SessionDataLoader : ALocalDataLoader<SessionData>
	{
		public override SessionData DefaultData => SessionData.Default;

		protected override SaveDataType _loadDataType => SaveDataType.Session;

		public SessionDataLoader(IDataCache<SessionData> cache) : base(cache)
		{
		}
	}
}
