using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Loading;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.PlayerController;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Scripts.Systems.Initialize
{
	public class InitializeDataLoader : IInitializable
	{
		[Inject] private IPlayerData _playerData;
		[Inject] private SceneContainer _sceneContainer;
		[Inject] private NetworkSystem _networkSystem;
		[Inject] private ProjectDataLoader _projectDataLoader;
		private float _lastProgress = 0.6f;


		public void Initialize()
		{
			RunLoaders().Forget();
		}

		private async UniTaskVoid RunLoaders()
		{
			UpdateJSLoaderProgress(_lastProgress);

			await _networkSystem.StartLoadUser();

			await foreach (var progress in _projectDataLoader.UpdateLoadingData())
			{
				UpdateJSLoaderProgress(_lastProgress + progress);
			}

			LoadingComplete();
		}


		private async void LoadingComplete()
		{
			UpdateJSLoaderProgress(0.91f);

			await Addressables.LoadSceneAsync("start", LoadSceneMode.Additive).ToUniTask();
			UpdateJSLoaderProgress(0.92f);
			await SceneManager.UnloadSceneAsync("Scenes/initial");
			UpdateJSLoaderProgress(0.93f);
			await Resources.UnloadUnusedAssets().ToUniTask();
			UpdateJSLoaderProgress(0.96f);
			GC.Collect();
			UpdateJSLoaderProgress(0.98f);
			await UniTask.WaitUntil(() => StartDataLoader.Instance);
			await StartDataLoader.Instance.LoadingComplete();


			await UniTask.Delay(1000);
			HideJSLoaderUIWithDelay();
		}

		private void UpdateJSLoaderProgress(float progress)
		{
			if (Application.isEditor)
			{
				Debug.Log($"JS LoadingUI progress update called {progress}");
			}
			else
			{
#if UNITY_WEBGL
				UpdateLoaderProgress(progress);
#endif
			}
		}

		private async void HideJSLoaderUIWithDelay(float delay = 0.3f)
		{
			UpdateJSLoaderProgress(1f);
			await UniTask.Delay(TimeSpan.FromSeconds(delay));

			if (Application.isEditor)
			{
				Debug.Log($"JS LoadingUI Hide called");
			}
			else
			{
#if UNITY_WEBGL
				HideLoaderUI();
#endif
			}
		}

#if UNITY_WEBGL
		[DllImport("__Internal")]
		public static extern void UpdateLoaderProgress(float progress);

		[DllImport("__Internal")]
		public static extern void HideLoaderUI();
#endif
	}
}