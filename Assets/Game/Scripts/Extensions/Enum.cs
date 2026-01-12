using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Extensions
{
	public class Enum<T> where T : struct, IConvertible
	{
		public static int Count
		{
			get
			{
				if (!typeof(T).IsEnum)
				{
					throw new ArgumentException("T must be an enumerated type.");
				}

				return Enum.GetNames(typeof(T)).Length;
			}
		}

		public static IEnumerable<T> GetValues()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		public static IEnumerable<T> GetValues(params T[] exept)
		{
			return Enum.GetValues(typeof(T)).Cast<T>().Except(exept);
		}

		/// <summary>
		/// Parse enum value.
		/// </summary>
		public static T Parse(string value, T defaultValue)
		{
			if (Enum.TryParse(value, true, out T result))
			{
				return result;
			}

			return defaultValue;
		}

		public static T PickRandom(params T[] except)
		{
			int count = Enum<T>.Count;

			if (count == 0)
			{
				return default(T);
			}

			var values = Enum<T>.GetValues();

			if (except != null)
			{
				int exceptIndexesCount = except.Length;
				int[] exceptIndexes = new int[exceptIndexesCount];
				int exceptIndexesIterator = 0;
				int valuesIterator = 0;
				foreach (var value in values)
				{
					for (int i = 0; i < exceptIndexesCount; i++)
					{
						if (except[i].Equals(value))
						{
							exceptIndexes[exceptIndexesIterator++] = valuesIterator;
							break;
						}
					}

					if (exceptIndexesIterator == exceptIndexesCount)
					{
						break;
					}

					valuesIterator++;
				}


				int startRandomIndex = UnityEngine.Random.Range(0, count);
				int randomIndex = startRandomIndex;
				while (true)
				{
					bool isExcept = false;
					int exceptLength = except.Length;
					for (int i = 0; i < exceptLength; i++)
					{
						if (exceptIndexes[i] == randomIndex)
						{
							isExcept = true;
							break;
						}
					}

					if (!isExcept)
					{
						return values.ElementAt(randomIndex);
					}

					if (++randomIndex == count)
					{
						randomIndex = 0;
					}

					if (randomIndex == startRandomIndex)
					{
						break;
					}
				}
			}

			return values.ElementAt(UnityEngine.Random.Range(0, count));
		}
	}
}