using System;
using Game.Scripts.Data;
using Game.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD.ResourceCollector
{
	[Serializable]
	public struct ResourceCollectTargetData
	{
		public ResourceType resourceType;
		public GameObject resourceIconPrefab;
		public Image image;
		public Transform root;
		public HUDChildPanel hud;
	}
}