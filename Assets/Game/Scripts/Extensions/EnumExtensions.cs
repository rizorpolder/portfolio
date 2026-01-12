using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts.Extensions
{
	public static class EnumExtensions
	{
		public static T Next<T>(this T enumValue, params T[] ignoreValues) where T : Enum
		{
			var enumValues = GetValues(ignoreValues);
			var currentIndex = enumValues.GetIndex(enumValue);

			var nextIndex = currentIndex + 1;
			return nextIndex >= enumValues.Length ? enumValues[0] : enumValues[nextIndex];
		}

		public static T Prev<T>(this T enumValue, params T[] ignoreValues) where T : Enum
		{
			var enumValues = GetValues(ignoreValues);
			var currentIndex = enumValues.GetIndex(enumValue);

			var prevIndex = currentIndex - 1;
			return prevIndex < 0 ? enumValues[enumValues.Length - 1] : enumValues[prevIndex];
		}

		private static T[] GetValues<T>(params T[] ignoreValues) where T : Enum
		{
			var enumValues = ((T[]) Enum.GetValues(typeof(T))).Except(ignoreValues).ToArray();
			if (enumValues.Length == 0)
			{
				throw new ArgumentException("Enum is empty");
			}

			return enumValues;
		}

		public static IEnumerable<Enum> GetFlags(this Enum e)
		{
			return Enum.GetValues(e.GetType()).Cast<Enum>().Where(e.HasFlag);
		}

		private static int GetIndex<T>(this T[] enumValues, T enumValue) where T : Enum
		{
			var currentIndex = Array.IndexOf(enumValues, enumValue);
			if (currentIndex == -1)
			{
				throw new ArgumentException("Source Enum value can't be ignored");
			}

			return currentIndex;
		}
	}
}