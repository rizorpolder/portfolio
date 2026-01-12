using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.Scripts.Systems.Sorting
{
	[RequireComponent(typeof(Collider2D))]
	public class DynamicSortingComparator : SortingComparator
	{
		[SerializeField] SortingGroup sortingGroup;
		[SerializeField] Transform model;

		private HashSet<SortingComparator> collisions = new HashSet<SortingComparator>();
		private int sortingOrder = 0;
		private Vector3 position = Vector3.zero;
		private bool needToUpdate = false;
		private float max = float.MaxValue;
		private float min = float.MinValue;

		protected virtual void OnCollisionEnter2D(Collision2D collision)
		{
			var sorting = collision.collider.gameObject.GetComponent<SortingComparator>();
			if (sorting != null)
			{
				collisions.Add(sorting);
				needToUpdate = true;
			}
		}

		protected virtual void OnCollisionExit2D(Collision2D collision)
		{
			var sorting = collision.collider.gameObject.GetComponent<SortingComparator>();
			if (sorting != null)
			{
				collisions.Remove(sorting);
			}
		}

		private void PreUpdate()
		{
			sortingOrder = 0;
			position = model.transform.position;
			position.z = 0;
			max = float.MaxValue;
			min = float.MinValue;
		}

		private void Update()
		{
			if (!needToUpdate)
				return;

			PreUpdate();
			foreach (var collision in collisions)
			{
				if (collision.Is2DObject)
				{
					var distance = GetDistance(collision);
					if (distance > 0 && distance < max && Mathf.Abs(min) > distance)
					{
						max = distance;
						sortingOrder = collision.sortingOrderLower;
					}

					if (distance < 0 && distance > min && Mathf.Abs(distance) < max)
					{
						min = distance;
						sortingOrder = collision.sortingOrderUpper;
					}
				}
				else
				{
					position.z += GetDistance(collision) > 0 ? 1 : 0;
				}
			}

			model.position = position;
			sortingGroup.sortingOrder = sortingOrder;

			if (collisions.Count == 0)
				needToUpdate = false;
		}

#if UNITY_EDITOR
		public override void OnDrawGizmos()
		{
			Vector3 testPosition = sortingGroup.transform.position;
			testPosition.z = 0;
			foreach (var collision in collisions)
			{
				if (collision.Is2DObject)
				{
					var distance = GetDistance(collision, out Vector3 contactPoint);
					var pos = collision.points[0];
					pos += collision.transform.position;
					Gizmos.color = Color.red;
					Gizmos.DrawLine(sortingGroup.transform.position, contactPoint);
					UnityEditor.Handles.Label(pos, $"{collision.name}: {distance}");
				}
			}
		}
#endif
	}
}