using Game.Scripts.Systems.Loading;
using Game.Scripts.UI.WindowsSystem;
using UI.HUD.ResourceCollector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Scripts.UI
{
	public class HUD : MonoBehaviour
	{
		private const string SCENE_KEY = "Scene";

		
		[SerializeField] private QuestPanel _questPanel;
		[SerializeField] private HUDPanel _leftPanel;
		[SerializeField] private PlayerPanel _playerPanel;
		[SerializeField] private ResourceCollectorBehaviour resourceCollectorBehaviour;

		public ResourceCollectorBehaviour ResourceCollectorBehaviour => resourceCollectorBehaviour;

		public PlayerPanel PlayerPanel => _playerPanel;
		private string _lastHUDHiderWindowName;

		[Inject] private WindowsController _windowsController;
		[Inject] private SceneLoaderManager _sceneLoaderManager;

		private void Start()
		{
			_windowsController.TopWindowChanged += WindowChangedHandler;
			_sceneLoaderManager.LoadingChanged += LoadingChangedHandler;
		}

		public void Show(string reason, bool left = true, bool quest = true)
		{
			if (left)
				_leftPanel.RemoveLock(reason);
			if (quest)
				_questPanel.RemoveLock(reason);
		}

		public void Hide(string reason, bool left = true, bool quest = true)
		{
			if (left)
				_leftPanel.AddLock(reason);
			if (quest)
				_questPanel.AddLock(reason);
		}

		private void WindowChangedHandler(int arg1, int arg2)
		{
			RefreshHUDMode();
		}

		private void RefreshHUDMode()
		{
			if (GetTopWindowInstance(out var topWindowInstance))
			{
				if (!string.IsNullOrEmpty(_lastHUDHiderWindowName))
				{
					if (_lastHUDHiderWindowName != topWindowInstance.Properties.name)
					{
						Show(_lastHUDHiderWindowName);
						_lastHUDHiderWindowName = null;
					}
				}

				if (!topWindowInstance.Properties.IsHideHUD)
					return;

				Hide(topWindowInstance.Properties.name);
				_lastHUDHiderWindowName = topWindowInstance.Properties.name;
			}
			else
			{
				if (string.IsNullOrEmpty(_lastHUDHiderWindowName))
					return;

				Show(_lastHUDHiderWindowName);
				_lastHUDHiderWindowName = null;
			}
		}

		private bool GetTopWindowInstance(out WindowInstance topWindowInstance)
		{
			topWindowInstance = null;
			if (!_windowsController.GetTopWindow(out BaseWindow topWindow))
				return false;

			return topWindow && _windowsController.GetWindowInstance(topWindow.ID, out topWindowInstance);
		}

		private void LoadingChangedHandler(bool obj)
		{
			var activeScene = SceneManager.GetActiveScene().name;
			
			if (SceneNames.IsOfficeScene(activeScene) || SceneNames.IsMentorsScene(activeScene))
			{
				Show(SCENE_KEY);
			}
			else
			{
				Hide(SCENE_KEY);
			}
		}
		

		private void OnDestroy()
		{
			_windowsController.TopWindowChanged -= WindowChangedHandler;
			_sceneLoaderManager.LoadingChanged -= LoadingChangedHandler;
		}
	}
}