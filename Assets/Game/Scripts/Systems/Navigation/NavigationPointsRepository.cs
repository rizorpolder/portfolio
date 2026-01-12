using System.Collections.Generic;
using Game.Navigation;
using UnityEngine;

namespace Game.Scripts.Systems.Navigation
{
	
	[CreateAssetMenu(fileName = "PointsRepository", menuName = "Project/Navigation/Points Repository")]

	public class NavigationPointsRepository : ScriptableObject
	{
		[SerializeField] private List<NavigationPointsGroup> groups;

		public Vector3 GetPoint(string name)
		{
			var buffer = name.Split('/');
			if (buffer.Length == 2)
			{
				var title = buffer[0];
				var token = buffer[1];

				var group = groups.Find(x => x.Title.Equals(title));
				if (group != null)
				{
					group.TryToGetPoint(token, out Vector3 point);
					return point;
				}
			}

			return Vector3.zero;
		}
	}
}