namespace Game.Scripts.Extensions
{
	public static class ArrayExtension
	{
		public static void Fill<T>(this T[] array, T value)
		{
			var length = array.Length;
			for (var i = 0; i < length; i++) array[i] = value;
		}

		public static T[] Add<T>(this T[] array, params T[] values)
		{
			if (array == null) array = new T[] { };

			if (values == null) values = new T[] { };

			var result = new T[array.Length + values.Length];
			array.CopyTo(result, 0);
			values.CopyTo(result, array.Length);

			return result;
		}

		public static T[] RemoveAt<T>(this T[] array, int index)
		{
			if (array == null) return new T[] { };
			if (index >= array.Length) return array;

			var result = new T[array.Length - 1];
			var offset = 0;
			for (var i = 0; i < result.Length; i++)
			{
				if (i == index) offset++;
				result[i] = array[i + offset];
			}

			return result;
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			var length = array.Length;
			for (var i = 0; i < length; i++)
			{
				var a = array[i];
				if (a != null && a.Equals(value)) return true;
			}

			return false;
		}

		public static void MoveElement<T>(T[] before, int oldIndex, int newIndex)
		{
			if (oldIndex == newIndex)
			{
				return;
			}

			var element = before[oldIndex];
			if (newIndex < oldIndex)
			{
				for (int i = oldIndex - 1; i >= newIndex; i--)
				{
					before[i + 1] = before[i];
				}

				before[newIndex] = element;
			}
			else
			{
				for (int i = oldIndex; i < newIndex; i++)
				{
					before[i] = before[i + 1];
				}

				before[newIndex] = element;
			}
		}
	}
}