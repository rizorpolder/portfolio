using UnityEngine;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	public class CharacterInfo
	{
		public int ID;
		public string Name;

		public CharacterType CharacterType;

		private bool _isPlayer => CharacterType == CharacterType.Player;
		public bool IsPlayer => _isPlayer;
		public bool IsNPC => !_isPlayer;

		public Sprite portrait;

		public CharacterInfo(int id, string nameInDatabase, CharacterType characterType, Sprite sprite)
		{
			ID = id;
			Name = nameInDatabase;
			CharacterType = characterType;
			portrait = sprite;
		}
	}
}