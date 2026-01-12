using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Google;
using UnityEditor;
using UnityEngine;

namespace Editor.Google
{
	[CustomEditor(typeof(AGoogleSheetRemoteConfig), true)]
	public class GoogleSheetRemoteConfigEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			if (target is AGoogleSheetRemoteConfig config)
			{
				if (GUILayout.Button("Sync From Google DB"))
				{
					Sync(config).Forget();
				}
			}

			base.OnInspectorGUI();
		}

		private async UniTask Sync(AGoogleSheetRemoteConfig config, Action onSuccess = null, Action onError = null)
		{
			var parsers = GoogleSheetParserFactory.GetParser(config);

			if (await TryParse(parsers))
			{
				onSuccess?.Invoke();
				Debug.Log("Successfully parsed Google Sheet remote config");
			}
			else
			{
				onError?.Invoke();
			}
		}

		private static async UniTask<bool> TryParse(IGoogleSheetParser parser)
		{
			bool completed = false;
			bool successful = false;
			parser?.Parse((success) =>
			{
				completed = true;
				if (success)
				{
					successful = true;
				}
				else
				{
					successful = false;
				}
			});

			await UniTask.WaitUntil(() => completed);
			return successful;
		}
	}
}