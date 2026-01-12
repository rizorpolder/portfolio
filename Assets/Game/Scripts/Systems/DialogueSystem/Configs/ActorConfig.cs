using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Systems.DialogueSystem
{
	[Serializable]
	public class ActorConfig
	{
		public int ID;

		public bool IsPlayer;
		public string ActorName;
		public string InitialPointName;
		public Vector3 InitialPositionVect;
		public Sprite Icon;
		public bool IsActive;
		public AssetReference AssetRef;
	}
}