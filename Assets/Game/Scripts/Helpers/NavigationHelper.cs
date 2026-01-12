using Game.Scripts.Systems.Character;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public static class NavigationHelper
	{
		public static Vector3 GetPositionOnNavMesh(Vector3 targetPosition, float maxDistance = 5)
		{
			UnityEngine.AI.NavMeshHit hit;
			if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition,
				    out hit,
				    maxDistance,
				    UnityEngine.AI.NavMesh.AllAreas))
			{
				return hit.position;
			}

			return targetPosition;
		}

		public static Vector3 GetRandomPositionInRadiusOnNavMesh(Vector3 destination, float radius = 1f)
		{
			var randomPoint = destination + Random.insideUnitSphere.normalized * radius;
			var nearNavMeshPoint = GetPositionOnNavMesh(randomPoint);
			return nearNavMeshPoint;
		}

		public static bool FindPath(Vector3 sourcePosition, Vector3 targetPosition, out UnityEngine.AI.NavMeshPath path)
		{
			path = new UnityEngine.AI.NavMeshPath();
			
			Debug.DrawLine(sourcePosition, targetPosition, Color.red, 2f);
			if (UnityEngine.AI.NavMesh.CalculatePath(sourcePosition, targetPosition, UnityEngine.AI.NavMesh.AllAreas, path))
			{
				return true;
			}

			return false;
		}
		
		public static bool IsReachablePathForAgent(CharacterNavigation navigation, Vector3 destination, out UnityEngine.AI.NavMeshPath path)
		{
			float characterRadius = navigation.Radius + 0.25f;
			var targetPosition = Vector3.MoveTowards(destination, navigation.transform.position, characterRadius);
			targetPosition = GetPositionOnNavMesh(targetPosition);

			var startPosition =  Vector3.MoveTowards(navigation.transform.position, destination, characterRadius);
			startPosition = GetPositionOnNavMesh(startPosition);

			return FindPath(startPosition, targetPosition, out path);
		}
	}
}