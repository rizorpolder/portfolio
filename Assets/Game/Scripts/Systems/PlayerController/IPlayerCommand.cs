using Game.Scripts.Data;
using Game.Scripts.Systems.PlayerController.Data;
using UnityEngine;

namespace Game.Scripts.Systems.PlayerController
{
	public interface IPlayerCommand
	{
		public void SetPlayerInfo(string name, string surname);
		public void SetPlayerSelectedIndex(int selectedIndex);
		public void SetPlayerGameComplete();
		public void SetTutorialCompleted(string currentStep);
		public void AddResource(Resource reward);
		public int GetResourceCount(Resource resource);
		public void SavePlayerPosition(string characterName, Vector3 position);
		public bool HavePlayerPositions(string characterID, out Vector3 position);
		public void ChangePlayerFloor(FloorNumber floorNumber);
		public PlayerData GetPlayerData();
	}
}