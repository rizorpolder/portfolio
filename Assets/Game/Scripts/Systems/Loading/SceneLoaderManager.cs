using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Initialize;
using Game.Scripts.UI.Loading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Scripts.Systems.Loading
{
	public class SceneLoaderManager
	{
		public Action<string> OnLoadSceneAction;
		public Action<string> OnAfterLoadSceneAction;
		public Action<string> OnUnloadSceneAction;
		public Action<string> OnAfterUnloadSceneAction;
		public event Action<bool> LoadingChanged;

		private LoadingScreen _loadingScreen;

		private bool _inProgress;
		[Inject] private SceneContainer _sceneContainer;
		[Inject] private SetupCoordinator _setupCoordinator;

		public bool InProgress
		{
			get => _inProgress;
			private set
			{
				_inProgress = value;
				LoadingChanged?.Invoke(_inProgress);
			}
		}

		private string _lastLoadedScene = String.Empty;

		public LoadingScreen Screen { get; }

		public SceneLoaderManager(LoadingScreen loadingScreen)
		{
			Screen = loadingScreen;
		}

		public async void Load(string sceneName, LoadingScreenType loadingType = LoadingScreenType.Default)
		{
			if (InProgress)
				return;

			InProgress = true;

			if (!Screen.gameObject.activeSelf)
			{
				Screen.gameObject.SetActive(true);

				// if (loadingType == LoadingScreenType.Default)
				// 	Screen.UpdateBackground();

				var animationCompleted = false;
				Screen.GetLoadingView(loadingType).ShowWithAnimation(() => { animationCompleted = true; });

				await UniTask.WaitUntil(() => animationCompleted);
			}

			// await UniTask.WaitUntil(() => Context.Meta.ActiveLand);

			//Context.Meta.ActiveLand.Root.SetActive(false);

			await LoadAsync(sceneName);

			var scene = SceneManager.GetSceneByName(sceneName);
			SceneManager.SetActiveScene(scene);

			InProgress = false;
			OnLoadSceneAction?.Invoke(sceneName);
			OnAfterLoadSceneAction?.Invoke(sceneName);
		}

		private async UniTask LoadAsync(string sceneName)
		{
			// var loadParam = sceneName != SceneNames.Office ? LoadSceneMode.Additive: LoadSceneMode.Single;
			// if (loadParam == LoadSceneMode.Single)
			_sceneContainer.RemoveAll();

			var operation = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);
			await operation;

			var sceneLogic = _sceneContainer.AddScene(operation);
			await sceneLogic.ActivateAsync();
		}

		public async void Unload(UnloadingTask unloadingTask)
		{
			if (InProgress)
				return;

			OnUnloadSceneAction?.Invoke(unloadingTask.SceneName);
			OnAfterUnloadSceneAction?.Invoke(unloadingTask.SceneName);
			InProgress = true;

			Screen.gameObject.SetActive(true);

			var animationCompleted = false;
			Screen.GetLoadingView(unloadingTask.LoadingScreenType).ShowWithAnimation(() =>
			{
				animationCompleted = true;
				unloadingTask.PostAnimationAction?.Invoke();
			});

			await UniTask.WaitUntil(() => animationCompleted);

			await UnloadSceneAsync(unloadingTask.SceneName);

			await UnityEngine.Resources.UnloadUnusedAssets();

			if (unloadingTask.HideLoadingView)
			{
				Screen.GetLoadingView(unloadingTask.LoadingScreenType).HideWithAnimation(() =>
				{
					Screen.gameObject.SetActive(false);
					InProgress = false;
					unloadingTask.Callback?.Invoke();
				});
			}
			else
			{
				InProgress = false;
				unloadingTask.Callback?.Invoke();
			}
		}

		private async UniTask UnloadSceneAsync(string sceneName)
		{
			var sceneLogic = _sceneContainer.RemoveScene(sceneName);
			if (sceneLogic is not null)
			{
				sceneLogic.Destroy();
			}
			else
			{
				await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
			}
		}

		public void FinishLoading()
		{
			var task = new UnloadingTask()
				.SetSceneName(SceneNames.Start)
				.SetLoadingScreenType(LoadingScreenType.Default)
				.SetHideLoadingView(false).AddCallback(() => { _setupCoordinator.GetCurrentSetup().Setup(); });

			Unload(task);
		}

		public void Hide()
		{
			InProgress = false;
			Screen.LastUsedView?.HideWithAnimation(() => { Screen.gameObject.SetActive(false); });
		}
	}
}