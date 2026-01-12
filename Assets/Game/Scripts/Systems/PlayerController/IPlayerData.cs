using Game.Scripts.Systems.PlayerController.Data;

namespace Game.Scripts.Systems.PlayerController
{
	public interface IPlayerData
	{
		public bool IsGameFinished { get; }
		public int PlayerSelectedIndexView { get; }
		public string PlayerID { get; }
		public FloorNumber PlayerFloorNumber { get; }
	}
}