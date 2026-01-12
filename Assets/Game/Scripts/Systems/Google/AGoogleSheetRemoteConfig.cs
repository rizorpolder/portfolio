using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Systems.Google
{
	public abstract class AGoogleSheetRemoteConfig : ScriptableObject
	{
		[SerializeField] private List<string> _googleSheetPublicUrls;
		public List<string> GoogleSheetPublicURLs() => _googleSheetPublicUrls;
		public abstract void Clear();
	}
}