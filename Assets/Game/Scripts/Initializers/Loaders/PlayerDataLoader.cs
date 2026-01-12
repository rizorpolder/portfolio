using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.PlayerController.Data;
using Game.Scripts.Systems.SaveSystem;
using Zenject;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public class PlayerDataLoader: ALocalDataLoader<PlayerData> 
	{
		public override PlayerData DefaultData => PlayerData.Default;

		protected override SaveDataType _loadDataType => SaveDataType.PlayerData;
		
		public PlayerDataLoader(IDataCache<PlayerData> cache) : base(cache)
		{
		}
		
	}
}