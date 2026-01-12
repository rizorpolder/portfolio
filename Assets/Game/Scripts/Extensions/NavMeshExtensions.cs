using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Extensions
{
	public static class NavMeshPathExtensions
	{
		public static float GetDistanceByCorners(this NavMeshPath path)
		{
			float distance = 0;
			for (var i = 0; i < path.corners.Length; i++)
			{
				var currentPoint = path.corners[i];
				if (path.corners.Length > i + 1)
				{
					var nextPoint = path.corners[i + 1];
					distance += Vector3.Distance(currentPoint, nextPoint);
				}
			}

			return distance;
		}

		public static bool TryGetPointByDistance(this NavMeshPath path, float distance, out Vector3 nearPoint)
		{
			nearPoint = Vector3.zero;
			if (path.status == NavMeshPathStatus.PathInvalid) return false;

			var corners = path.corners.ToList();
			var fullDistance = 0f;

			var prevCorner = corners.First();
			for (var i = 1; i < corners.Count; i++)
			{
				var corner = corners[i];
				fullDistance += Vector3.Distance(prevCorner, corner);
				if (fullDistance > distance)
				{
					nearPoint = prevCorner;
					return true;
				}

				prevCorner = corner;
			}

			nearPoint = corners.Last();
			return true;
		}

		public static float GetPathDistance(this NavMeshPath path)
		{
			var prevCorner = path.corners.First();
			var distance = 0f;
			for (var i = 1; i < path.corners.Length; i++)
			{
				var corner = path.corners[i];
				distance += Vector3.Distance(prevCorner, corner);
				prevCorner = corner;
			}

			return distance;
		}

		public static float GetPathRemainingDistance(this NavMeshAgent navMeshAgent)
		{
			if (navMeshAgent.pathPending ||
			    navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
			    navMeshAgent.path.corners.Length == 0)
				return navMeshAgent.remainingDistance;

			float distance = 0.0f;
			for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
			{
				distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
			}

			return distance;
		}
	}
}