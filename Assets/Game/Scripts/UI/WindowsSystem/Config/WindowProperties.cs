using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.UI.WindowsSystem
{
	[Serializable]
	public class WindowProperties
	{
		public string name;

		public WindowType type;

		public AssetReference assetReference;
		public int priority;

		public bool IsHideOtherWindows = false;

		public bool IsShowHiddenWindowsOnEndAnimations = false;

		public bool IsHideHUD = true;

		public bool IsHasShadow = true;
		public bool IsHideShadowOnEndAnimation;
		public bool IsOverrideShadowColor = false;
		public Color ShadowColor = Color.black;

		public bool IsShadowUpperHUD = true;

		public bool IsCached = false;
	}
}