using System;
using System.Collections.Generic;
using Editor.Google;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.Texts.Configs;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Editor
{
#if UNITY_EDITOR

	[CustomEditor(typeof(TextsRepositoryConfig))]
	public class TextRepositoryConfigEditor : GoogleSheetRemoteConfigEditor
	{
		private TextsRepositoryConfig _config;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();


			_config = (TextsRepositoryConfig) target;
			if (GUILayout.Button("Send to server"))
			{
				var data = new TextsData();

				foreach (var group in _config._textGroups)
				{
					foreach (var textEntry in group.TextEntries)
					{
						var item = new TextItem();
						item.type = group.GroupType.ToString();
						item.token = textEntry.Key;
						item.text = textEntry.Value;
						data.items.Add(item);
					}
				}

				var result = JsonConvert.SerializeObject(data);
				SendRequestToServer(result);
			}
		}

		private async void SendRequestToServer(string data)
		{
			var config = (NetworkConfig) AssetDatabase.LoadAssetAtPath(
				$"Assets/Game/Configs/NetworkConfig.asset",
				typeof(NetworkConfig));
			var path = config.GetRestApiUri();
		
			var request = new UnityWebRequest($"{path}texts/replace-all", "POST");
			var requestData = System.Text.Encoding.UTF8.GetBytes(data);
			var uploadHandler = new UploadHandlerRaw(requestData);
			request.uploadHandler = uploadHandler;
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");

			try
			{
				await request.SendWebRequest();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Debug.Log(request.isDone ? "Request completed" : "Request error");
		}
	}

	[Serializable]
	internal class TextsData
	{
		public List<TextItem> items = new List<TextItem>();
	}

	[Serializable]
	internal class TextItem
	{
		public string token;
		public string type;
		public string text;
	}
#endif
}