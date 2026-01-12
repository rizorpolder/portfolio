using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Extensions;
using Game.Scripts.Systems.Character;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Scripts.Helpers
{
	public static class CharacterNavigationExtension
	{
		/// <summary>
		/// Снизу имеем дополнительное пространство, т.к. root transform Люси у ее ног
		/// </summary>
		private static Vector3 ViewportMinOffset = new Vector3(0, -0.8f);

		/// <summary>
		/// Насколько нужно увеличить зону вьюпорта
		/// </summary>
		private static float ViewportXScale = 1.12f;

		private static float ViewportCinemtaicYScale = 0.8f;
		private static float ViewportYScale = 1.12f;

		public static float MinDistanceToTeleport = 3f;

		public static MovementBuilder CreateMovement(this Character character, Vector3 destination)
		{
			var queue = new Queue<Vector3>();
			queue.Enqueue(destination);

			return character.CreateMovement(queue);
		}

		public static MovementBuilder CreateMovement(this Character character, Queue<Vector3> points)
		{
			var builder = new MovementBuilder(character, points);
			return builder;
		}

		/// <summary>
		/// Расчитываем путь к точке назначения
		/// </summary>
		/// <param name="characterEntity"></param>
		/// <param name="destination"></param>
		/// <returns>Можем ли пройти до назначения</returns>
		public static bool CalculatePath(this Character characterEntity, Vector3 destination, out NavMeshPath path)
		{
			path = new NavMeshPath();

			if (characterEntity.Navigation)
			{
				characterEntity.Navigation.SwitchToAgent();
				var canMove = characterEntity.Navigation.CalculatePath(destination, path) &&
				              path.status == NavMeshPathStatus.PathComplete;
				characterEntity.Navigation.SwitchToObstacle();
				return canMove;
			}

			return false;
		}

		public static void SetPosition(this Character character,
			Vector3 destination,
			Action<MoveCompleteStatus> callback = null)
		{
			character.CreateMovement(destination)
				.Instantly()
				.OnMoveComplete(callback)
				.Move();
		}

		public static bool CanWarpToViewport(this Character characterEntity,
			NavMeshPath path,
			bool cinemtaic,
			out Vector3 position)
		{
			position = Vector3.zero;

			if (path.corners.Length < 2)
			{
				Debug.LogWarning($"can not warp unit {characterEntity.Name} bad path for warp to viewport");
				return false;
			}

			if (characterEntity.View && characterEntity.View is CharacterView characterView)
			{
				var reversedPath = path.corners.Reverse().ToArray();
				var prevCorner = reversedPath[0].WithZ(0);

				var cameraBounds = Camera.main.GetOrthographicBounds();
				cameraBounds.size = new Vector3(cameraBounds.size.x * ViewportXScale,
					cameraBounds.size.y * (cinemtaic ? ViewportCinemtaicYScale : ViewportYScale),
					1f);
				cameraBounds.min += ViewportMinOffset;

				for (int i = 1; i < reversedPath.Length; i++)
				{
					var corner = reversedPath[i].WithZ(0);

					if (!cameraBounds.Contains(corner) && !characterView.IsVisible)
					{
						var direction = (prevCorner - corner).normalized;
						if (!direction.Equals(Vector3.zero))
						{
							var ray = new Ray(corner, direction);

							if (cameraBounds.IntersectRay(ray, out var distance))
							{
								position = ray.GetPoint(distance);

								return true;
							}
						}

						break;
					}

					prevCorner = corner;
				}
			}

			return false;
		}

		public static void WarpToViewPortAndMoveTo(this Character characterEntity,
			Vector3 destination,
			Action<MoveCompleteStatus> onMoveComplete = null)
		{
			var movement = characterEntity.CreateMovement(destination);
			movement.OnMoveComplete(onMoveComplete)
				//.WarpToViewport()
				.Move();
		}

		public static void TeleportTo(this Character character,
			Vector3 destination,
			int angle,
			Action onMoveComplete = null)
		{
			var path = new NavMeshPath();
			character.Navigation.SwitchToAgent();
			character.Navigation.CalculatePath(destination, path);


			character.CreateMovement(destination)
				.OnMoveComplete(status => { onMoveComplete?.Invoke(); })
				.Move();
		}
	}
}