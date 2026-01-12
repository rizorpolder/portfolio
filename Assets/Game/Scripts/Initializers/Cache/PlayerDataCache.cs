using Game.Scripts.Systems.PlayerController.Data;

namespace Game.Scripts.Systems.Initialize.Cache
{
	public class PlayerDataCache : IDataCache<PlayerData>
	{
		private PlayerData _data;

		void IDataCache<PlayerData>.SetData(PlayerData data) => _data = data;
		public PlayerData GetData() => _data ?? PlayerData.Default;
	}
}