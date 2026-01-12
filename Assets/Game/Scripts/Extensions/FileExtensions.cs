using System;
using System.IO;
using UnityEngine;

namespace Game.Scripts.Extensions
{
	public static class FileExtensions
	{
		public static T Load<T>(string path, T defaultValue)
		{
			var data = defaultValue;
			if (File.Exists(path))
			{
				try
				{
					var parseResult = JsonUtility.FromJson<T>(File.ReadAllText(path));
					if (parseResult != null)
						data = parseResult;
				}
				catch (Exception e)
				{
					Debug.LogError($"Failed to parse {path}\nException:{e}");
				}
			}

			return data;
		}

		public static string LoadString(string path, string defaultValue)
		{
			var data = defaultValue;
			if (File.Exists(path))
			{
				try
				{
					data = File.ReadAllText(path);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}

			return data;
		}

		public static string Load(string path)
		{
			var data = string.Empty;
			if (File.Exists(path))
			{
				try
				{
					data = File.ReadAllText(path);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}

			return data;
		}

		public static void Save<T>(T data, string path) where T : class
		{
			var json = JsonUtility.ToJson(data);
			File.WriteAllText(path, json);
		}

		public static void Save(string data, string path)
		{
			File.WriteAllText(path, data);
		}

		public static void CreateDirIfNotExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		public static bool FileExists(string path)
		{
			return File.Exists(path);
		}
	}
}