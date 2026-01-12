using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Data;
using UnityEngine;

namespace Game.Scripts.UI.Configs
{
	[CreateAssetMenu(fileName = "ResourcesSpriteRepository", menuName = "Project/Resources Sprite Repository")]
	public class ResourcesSpriteRepository : ScriptableObject
	{
		public List<ResourceInfo> Resources = new List<ResourceInfo>();

		public Sprite GetResourceSprite(ResourceType resourceType)
		{
			return Resources.FirstOrDefault(x => x.Type.Equals(resourceType)).icon;
		}

		public Sprite GetResourceSprite(Resource resource)
		{
			return Resources.FirstOrDefault(x => x.Type.Equals(resource.Type)).icon;
		}
	}

	[Serializable]
	public class ResourceInfo
	{
		public ResourceType Type;
		public UnityEngine.Sprite icon;
		public UnityEngine.Sprite lockedIcon;
	}
}