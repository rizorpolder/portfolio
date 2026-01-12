using System;
using System.Globalization;
using UnityEngine;

namespace Game.Scripts.Extensions
{
	public static class VectorExtension
	{
		public static bool TryToParse(this ref Vector3 vector, string value)
		{
			// лишняя операция из-за того, что ToString сохраняет с этими знаками
			value = value.Trim('(', ')');

			string[] symb = value.Split(',');

			if (symb.Length <= 2)
				return false;

			vector.x = float.Parse(symb[0], CultureInfo.InvariantCulture.NumberFormat);
			vector.y = float.Parse(symb[1], CultureInfo.InvariantCulture.NumberFormat);
			vector.z = float.Parse(symb[2], CultureInfo.InvariantCulture.NumberFormat);
			return true;
		}
		
		public static bool TryToParse(this ref Vector2 vector, string value)
		{
			// лишняя операция из-за того, что ToString сохраняет с этими знаками
			value = value.Trim('(', ')');

			string[] symb = value.Split(',');
			
			if (symb.Length >= 3)
				return false;

			vector.x = float.Parse(symb[0], CultureInfo.InvariantCulture.NumberFormat);
			vector.y = float.Parse(symb[1], CultureInfo.InvariantCulture.NumberFormat);
			return true;
		}

		public static Vector2 ToVector2(this Vector3 vector3)
		{
			return new Vector2(vector3.x, vector3.y);
		}
		
		public static Vector3 ToVector3(this Vector2 vector2)
		{
			return new Vector3(vector2.x, vector2.y, 0);
		}

		public static Vector3 WithZ(this Vector3 vector3, int z)
		{
			return new Vector3(vector3.x, vector3.y, z);
		}

		public static float FindDegree(this Vector3 vector2)
		{
			var value = (float) (Mathf.Atan2(vector2.x, vector2.y) / Math.PI * 180f);
			return value < 0 ? value + 360 : value;
		}

		public static float FindEllipseDegree(this Vector3 point, float a, float b)
		{
			return FindEllipseDegree(point.ToVector2(), a, b);
		}

		public static float GetCircleDegree(this Vector3 direction)
		{
			return GetCircleDegree(direction.ToVector2());
		}

		public static float GetCircleDegree(this Vector2 direction)
		{
			return FindEllipseDegree(direction, 1, 1);
		}

		public static float FindEllipseDegree(this Vector2 point, float a, float b)
		{
			var value = Mathf.Atan2(point.x * a, point.y * b) * (180f / Mathf.PI);
			return value < 0 ? value + 360 : value;
		}

		public static float GetDistanceToLine(this Vector3 target, Vector3 linePointA, Vector3 linePointB)
		{
			return target.GetDistanceToLine(linePointA, linePointB, out var contactPoint);
		}

		public static float GetDistanceToLine(this Vector3 target,
			Vector3 linePointA,
			Vector3 linePointB,
			out Vector3 contactPoint)
		{
			#region Дистанция через скалярное произведение векторов

			Vector2 v = linePointB - linePointA;
			Vector2 w = target - linePointA;

			//левее
			float c1 = Dot(w, v);
			if (c1 <= 0)
			{
				contactPoint = linePointA;
				return Normalize(target, linePointA) * Math.Sign(target.y - linePointA.y);
			}

			//правее
			float c2 = Dot(v, v);
			if (c2 <= c1)
			{
				contactPoint = linePointB;
				return Normalize(target, linePointB) * Math.Sign(target.y - linePointB.y);
			}

			float d = c1 / c2;
			Vector2 Pb = (Vector2) linePointA + d * v;
			contactPoint = Pb;
			return Normalize(target, Pb) * Math.Sign(target.y - Pb.y);

			#endregion
		}

		private static float Dot(Vector2 u, Vector2 v)
		{
			var temp = ((u).x * (v).x + (u).y * (v).y);
			return temp;
		}

		private static float Normalize(Vector2 v)
		{
			return Mathf.Sqrt(Dot(v, v));
		}

		private static float Normalize(Vector2 u, Vector2 v)
		{
			return Normalize(u - v);
		}
	}
}