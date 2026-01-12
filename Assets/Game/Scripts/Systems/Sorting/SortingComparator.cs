using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Systems.Sorting
{
	public class SortingComparator : MonoBehaviour
	{
		public enum SortingType
		{
			Point,
			Line
		}

		#region data

		public SortingType sortingType;
		public List<Vector3> points = new List<Vector3>();
		public bool Is2DObject = true;
		public int sortingOrderUpper = 1;
		public int sortingOrderLower = -1;

		#endregion

		private float GetPointsDistance(Vector3 point1, Vector3 point2)
		{
			return point1.y - point2.y;
		}

		private float GetLineDistance(Vector3 point,
			Vector3 linePoint0,
			Vector3 linePoint1,
			out Vector3 contactPoint)
		{
			#region Дистанция через скалярное произведение векторов

			Vector2 v = linePoint1 - linePoint0;
			Vector2 w = point - linePoint0;

			//перс левее
			float c1 = Dot(w, v);
			if (c1 <= 0)
			{
				contactPoint = linePoint0;
				return Normalize(point, linePoint0) * Math.Sign(point.y - linePoint0.y);
			}

			//перс правее
			float c2 = Dot(v, v);
			if (c2 <= c1)
			{
				contactPoint = linePoint1;
				return Normalize(point, linePoint1) * Math.Sign(point.y - linePoint1.y);
			}

			float d = c1 / c2;
			Vector2 Pb = (Vector2) linePoint0 + d * v;
			contactPoint = Pb;
			return Normalize(point, Pb) * Math.Sign(point.y - Pb.y);

			#endregion
		}

		private float Dot(Vector2 u, Vector2 v)
		{
			var temp = ((u).x * (v).x + (u).y * (v).y);
			return temp;
		}

		private float Normalize(Vector2 v)
		{
			return Mathf.Sqrt(Dot(v, v));
		}

		private float Normalize(Vector2 u, Vector2 v)
		{
			return Normalize(u - v);
		}

		public float GetDistance(object obj)
		{
			return GetDistance(obj, out var contactPoint);
		}

		public float GetDistance(object obj, out Vector3 contactPoint)
		{
			SortingComparator test = obj as SortingComparator;
			contactPoint = points[0];
			if (sortingType == SortingType.Point && test.sortingType == SortingType.Point)
			{
				return GetPointsDistance(points[0] + transform.position, test.points[0] + test.transform.position);
			}
			else if (sortingType == SortingType.Point && test.sortingType == SortingType.Line)
			{
				return GetLineDistance(points[0] + transform.position,
					test.points[0] + test.transform.position,
					test.points[1] + test.transform.position,
					out contactPoint);
			}
			else if (sortingType == SortingType.Line && test.sortingType == SortingType.Point)
			{
				return GetLineDistance(test.points[0] + test.transform.position,
					points[0] + transform.position,
					points[1] + transform.position,
					out contactPoint);
			}

			return 0;
		}

		public void MovePoint(int i, Vector2 point)
		{
			points[i] = point;
		}

#if UNITY_EDITOR
		public virtual void OnDrawGizmos()
		{
			if (sortingType == SortingType.Point)
			{
				if (points.Count > 0)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(points[0] + transform.position, 0.07f);
				}
			}
			else
			{
				if (points != null && points.Count > 1)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(points[0] + transform.position, points[1] + transform.position);
				}
			}
		}
#endif
	}
}