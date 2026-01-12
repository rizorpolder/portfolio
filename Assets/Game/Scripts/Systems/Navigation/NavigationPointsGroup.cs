using System.Collections.Generic;
using Game.Scripts.Systems.Navigation;
using UnityEngine;

namespace Game.Navigation
{
	[CreateAssetMenu(fileName = "NavigationGroup", menuName = "Project/Navigation/Navigation Group")]
	public class NavigationPointsGroup : ScriptableObject
	{
		[SerializeField] private Color textColor = Color.green;
		[SerializeField] private string title;

		public string Title
		{
			get { return title; }
		}

		[SerializeField] private List<NavigationPoint> points = new List<NavigationPoint>();

		public List<NavigationPoint> Points
		{
			get { return points; }
			set { points = value; }
		}

		public Color TextColor => this.textColor;

		public bool TryToGetPoint(string name, out Vector3 result)
		{
			var point = points.Find(x => x.Name.Equals(name));
			if (point != null)
			{
				result = point.Position;
			}
			else
			{
				if (Application.isEditor)
					Debug.LogWarning($"Point not found: {name}");

				result = Vector3.zero;
			}

			return result != null;
		}

		public bool TryToGetPoint(string name, out NavigationPoint result)
		{
			var point = points.Find(x => x.Name.Equals(name));
			if (point != null)
			{
				result = point;
			}
			else
			{
				result = null;
			}

			return result != null;
		}

		public int Count()
		{
			return points.Count;
		}

		public bool Contain(string name)
		{
			var point = points.Find(x => x.Name.Equals(name));
			return point != null;
		}

		public void Remove(NavigationPoint point)
		{
			points.Remove(point);
		}

		public void Add(NavigationPoint point)
		{
			if (TryToGetPoint(point.Name, out NavigationPoint result))
			{
				result.Position = point.Position;
			}
			else
			{
				points.Add(point);
			}
		}
	}
}