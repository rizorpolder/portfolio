using System;
using System.IO;
using Game.Scripts.Systems.SaveSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
	public class CustomMenu
	{
		[MenuItem("Project/Clear all prefs", priority = 1)]
		public static void ClearPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();

			var path = Application.persistentDataPath;
			foreach (SaveDataType data in Enum.GetValues(typeof(SaveDataType)))
			{
				if (File.Exists($"{path}/{data}"))
				{
					File.Delete($"{path}/{data}");
				}
			}
			
			if (File.Exists($"{path}/MetaData"))
			{
				File.Delete($"{path}/MetaData");
			}
		}

		[MenuItem("Project/Clear credentials", priority = 2)]
		public static void ClearCredentials()
		{
		}
	}
}