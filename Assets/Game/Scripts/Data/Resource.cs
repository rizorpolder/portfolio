using System;
using System.Collections.Generic;

namespace Game.Scripts.Data
{
	public enum ResourceType
	{
		Free = 0,
		Points = 1,
	}

	[Serializable]
	public class Resource : ICloneable
	{
		public ResourceType Type;
		public int Value;

		public Resource()
		{
			Type = ResourceType.Points;
			Value = 0;
		}

		public Resource(ResourceType type, int value)
		{
			Type = type;
			Value = value;
		}

		protected bool Equals(Resource other)
		{
			return Type == other.Type && Value.Equals(other.Value);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) Type;
				hashCode = (hashCode * 397) ^ Value.GetHashCode();
				return hashCode;
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public override string ToString()
		{
			return $"{Type}_{Value}";
		}
	}

	[Serializable]
	public class ResourceList : ICloneable
	{
		public List<Resource> Resources = new List<Resource>();

		public object Clone()
		{
			var clone = new List<Resource>();
			foreach (var resource in Resources)
			{
				clone.Add((Resource) resource.Clone());
			}

			return new ResourceList()
			{
				Resources = clone,
			};
		}

		public void Append(ResourceList resourceList)
		{
			if (resourceList != null && resourceList.Resources != null)
			{
				Resources.AddRange(resourceList.Resources);
			}
		}
	}
}