using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Extensions
{
	public static class IEnumerableExtensions
	{
		public static T FindMax<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
		{
			if (source == null)
				throw new ArgumentNullException("source");

			bool first = true;
			T maxObj = default(T);
			U maxKey = default(U);
			foreach (var item in source)
			{
				if (first)
				{
					maxObj = item;
					maxKey = selector(maxObj);
					first = false;
				}
				else
				{
					U currentKey = selector(item);
					if (currentKey.CompareTo(maxKey) > 0)
					{
						maxKey = currentKey;
						maxObj = item;
					}
				}
			}

			if (first)
				throw new InvalidOperationException("Sequence is empty.");

			return maxObj;
		}

		public static T FindMin<T, U>(this IEnumerable<T> source, Func<T, U> selector) where U : IComparable<U>
		{
			if (source == null)
				throw new ArgumentNullException("source");

			bool first = true;
			T maxObj = default(T);
			U maxKey = default(U);
			foreach (var item in source)
			{
				if (first)
				{
					maxObj = item;
					maxKey = selector(maxObj);
					first = false;
				}
				else
				{
					U currentKey = selector(item);
					if (currentKey.CompareTo(maxKey) < 0)
					{
						maxKey = currentKey;
						maxObj = item;
					}
				}
			}

			if (first)
				throw new InvalidOperationException("Sequence is empty.");

			return maxObj;
		}

		private static Random rng = new Random();

		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static int GetRandomIndexByWeights(this IEnumerable<IWeight> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			float accumulatedWeight = 0f;

			foreach (var item in source)
			{
				accumulatedWeight += item.GetWeight();
			}

			float rand = UnityEngine.Random.Range(0f, accumulatedWeight);

			float accumulator = 0f;
			int index = 0;
			foreach (var item in source)
			{
				if ((rand - accumulator) <= item.GetWeight())
				{
					return index;
				}

				accumulator += item.GetWeight();
				++index;
			}

			return index;
		}

		public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			if (comparison == null)
				throw new ArgumentNullException("comparison");

			int count = list.Count;
			for (int j = 1; j < count; j++)
			{
				T key = list[j];

				int i = j - 1;
				for (; i >= 0 && comparison(list[i], key) > 0; i--)
				{
					list[i + 1] = list[i];
				}

				list[i + 1] = key;
			}
		}

		#region Median

		/// <summary>
		/// Partitions the given list around a pivot element such that all elements on left of pivot are <= pivot
		/// and the ones at thr right are > pivot. This method can be used for sorting, N-order statistics such as
		/// as median finding algorithms.
		/// Pivot is selected ranodmly if random number generator is supplied else its selected as last element in the list.
		/// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 171
		/// </summary>
		private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null)
			where T : IComparable<T>
		{
			if (rnd != null)
				list.Swap(end, rnd.Next(start, end + 1));

			var pivot = list[end];
			var lastLow = start - 1;
			for (var i = start; i < end; i++)
			{
				if (list[i].CompareTo(pivot) <= 0)
					list.Swap(i, ++lastLow);
			}

			list.Swap(end, ++lastLow);
			return lastLow;
		}

		/// <summary>
		/// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
		/// Note: specified list would be mutated in the process.
		/// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
		/// </summary>
		public static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
		{
			return NthOrderStatistic(list, n, 0, list.Count - 1, rnd);
		}

		private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd)
			where T : IComparable<T>
		{
			while (true)
			{
				var pivotIndex = list.Partition(start, end, rnd);
				if (pivotIndex == n)
					return list[pivotIndex];

				if (n < pivotIndex)
					end = pivotIndex - 1;
				else
					start = pivotIndex + 1;
			}
		}

		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			if (i == j) //This check is not required but Partition function may make many calls so its for perf reason
				return;
			(list[i], list[j]) = (list[j], list[i]);
		}

		/// <summary>
		/// Note: specified list would be mutated in the process.
		/// </summary>
		public static T Median<T>(this IList<T> list) where T : IComparable<T>
		{
			return list.NthOrderStatistic((list.Count - 1) / 2);
		}

		public static double Median<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
		{
			var list = sequence.Select(getValue).ToList();
			var mid = (list.Count - 1) / 2;
			return list.NthOrderStatistic(mid);
		}

		#endregion
	}
}