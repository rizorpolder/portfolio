using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Systems.DialogueSystem.Data
{
	[Serializable]
	public class Actor : Asset
	{
		private Sprite _sprite;
		private CharacterType _characterType;
		private string _initialPoint;
		private Vector2 _initialPointVector;
		private bool _isActive = false;
		private string _assetRefGUID;
		public string InitialPoint => _initialPoint;
		public CharacterType CharacterType => _characterType;
		public bool IsActive => _isActive;

		public string AssetRefGUID => _assetRefGUID;
		
		
		public Actor(Actor sourceAsset) : base(sourceAsset)
		{
			_sprite = sourceAsset._sprite;
			_characterType = sourceAsset._characterType;
			_isActive = sourceAsset._isActive;
			_initialPoint = sourceAsset._initialPoint;
			_initialPointVector = sourceAsset._initialPointVector;
			_isActive = sourceAsset.IsActive;
			_assetRefGUID = sourceAsset._assetRefGUID;
		}

		public Actor(ActorConfig source) : base(source.ID, source.ActorName)
		{
			_sprite = source.Icon;
			_characterType = source.IsPlayer ? CharacterType.Player : CharacterType.NPC;
			_initialPoint = source.InitialPointName;
			_initialPointVector = source.InitialPositionVect;
			_isActive = source.IsActive;
			_assetRefGUID = source.AssetRef.AssetGUID;
		}

		public Sprite GetPortraitSprite() => _sprite;
		public Sprite SetPortraitSprite(Sprite sprite) => _sprite = sprite;
		
		public void UpdateAssetRefGuid(string referenceGUID) => _assetRefGUID = referenceGUID;
		
	}
}