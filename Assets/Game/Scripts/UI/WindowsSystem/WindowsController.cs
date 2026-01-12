using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.Loading;
using Game.Scripts.UI.Common;
using Game.Scripts.UI.Common.Animation;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.WindowsSystem
{
	public class WindowsController : MonoBehaviour
	{
		public event Action<int, int> TopWindowChanged;

		[SerializeField] private Transform parent;
		[SerializeField] private ShadowAnimation shadow;

		private int _topWindow;

		#region Props

		public int TopWindow
		{
			get => _topWindow;
			private set
			{
				var previousTopWindow = _topWindow;
				_topWindow = value;
				TopWindowChanged?.Invoke(previousTopWindow, _topWindow);
			}
		}

		public bool HasActiveWindows => activeWindows.Count > 0;
		public bool HasActiveOrInProcessWindow => activeWindows.Count > 0 || createdWindow.Count > 0;
		public bool HasCreatedWindow => createdWindow.Count > 0;

		public bool HasAnyWindow => HasActiveOrInProcessWindow || waitedWindows.Count > 0;
		public bool HasClosingProcessWindows => closingWindows.Count > 0;

		#endregion

		[Inject] private SceneLoaderManager _sceneLoaderManager;

		private void Start()
		{
			_sceneLoaderManager.OnLoadSceneAction += OnLoadSceneAction;
		}

		private void OnLoadSceneAction(string obj)
		{
			if (HasActiveOrInProcessWindow)
				HideAllWindows();
		}

		// список открытых окон
		private List<WindowInstance> activeWindows = new List<WindowInstance>();
		private List<WindowInstance> createdWindow = new List<WindowInstance>();
		private List<WindowInstance> closingWindows = new List<WindowInstance>();

		// список окон в режиме ожидания
		private SortedDictionary<WindowInstance, Action<BaseWindow>> waitedWindows =
			new SortedDictionary<WindowInstance, Action<BaseWindow>>();

		// корутина ожидания для waitedWindows
		private IEnumerator waitToShowCoroutine = null;

		[Inject] private WindowsFactory _factory;
		[Inject] private DiContainer _container;

		public void Show(WindowType windowType,
			Action<BaseWindow> callback = null,
			WindowSource source = WindowSource.Logic)
		{
			// блокируем запуск других окон при нажатии на кнопки, если у нас есть уже одно окно в процесс открывания
			if (source.Equals(WindowSource.Button))
				if (createdWindow.Count > 0 || IsWindowInProcess())
					return;

			if (!_factory.GetWindow(windowType, out var window))
			{
				Debug.LogErrorFormat($"No window found with type: {windowType}");
				return;
			}

			Show(window, callback);
		}

		public void Show(string windowName,
			Action<BaseWindow> callback = null,
			WindowSource source = WindowSource.Logic)
		{
			// блокируем запуск других окон при нажатии на кнопки, если у нас есть уже одно окно в процесс открывания
			if (source.Equals(WindowSource.Button))
				if (createdWindow.Count > 0 || IsWindowInProcess())
					return;

			if (!_factory.GetWindow(windowName, out var window))
			{
				Debug.LogErrorFormat($"No window found with name: {windowName}");
				return;
			}

			Show(window, callback);
		}

		private void Show(WindowInstance window, Action<BaseWindow> callback = null)
		{
			if (activeWindows.Contains(window) || createdWindow.Contains(window))
				return;

			if (!IsCanShowWindow(window))
			{
				if (!waitedWindows.ContainsKey(window))
				{
					AddToWait(window, callback);
				}

				return;
			}

			waitedWindows.Remove(window);

			createdWindow.Add(window);

			_factory.CreateWindow(window,
				parent,
				() =>
				{
					createdWindow.Remove(window);
					OnShowAction(window);
					callback?.Invoke(window.Instance);
				});
		}

		private void OnShowAction(WindowInstance windowInstance)
		{
			windowInstance.Instance.Show();

			windowInstance.Instance.transform.SetAsLastSibling();

			if (windowInstance.Properties.IsHasShadow)
			{
				if (windowInstance.Properties.IsOverrideShadowColor)
					shadow.SetActiveColor(windowInstance.Properties.ShadowColor);
				else
					shadow.ResetColor();

				shadow.Show();
				var newIndex = windowInstance.Instance.transform.GetSiblingIndex() - 1;
				shadow.transform.SetSiblingIndex(newIndex < 0 ? 0 : newIndex);
				shadow.ChangeCanvasState(!windowInstance.Properties.IsShadowUpperHUD);
			}
			else
			{
				shadow.Hide();
			}

			if (windowInstance.Properties.IsHideOtherWindows)
			{
				SetActiveAllWindows(false);
			}

			activeWindows.Add(windowInstance);
			TopWindow = (int) windowInstance.Properties.type;
		}

		private void AddToWait(WindowInstance window, Action<BaseWindow> callback)
		{
			Debug.LogErrorFormat($"Can't show window now: {window.Properties.name}");
			waitedWindows.Add(window, callback);

			if (waitToShowCoroutine == null)
			{
				waitToShowCoroutine = WaitToShow();
				StartCoroutine(waitToShowCoroutine);
			}
		}

		IEnumerator WaitToShow()
		{
			while (waitedWindows.Count > 0)
			{
				yield return new WaitForSeconds(0.05f);
				if (waitedWindows.Count > 0)
					Show(waitedWindows.First().Key, waitedWindows.First().Value);
			}

			waitToShowCoroutine = null;
			yield return null;
		}

		public void HideAllWindows()
		{
			if (waitToShowCoroutine != null)
			{
				StopCoroutine(waitToShowCoroutine);
				waitToShowCoroutine = null;
				waitedWindows.Clear();
			}

			int max = 0;
			while (activeWindows.Count > 0 && max < 10)
			{
				var window = activeWindows[0];
				if (!window.Instance.IsShown())
				{
					window.Instance.Hide();
					OnHideAction(window);
					OnHiddenAction(window);
					ClosingComplete(window);
				}

				++max;
			}

			max = 0;
			while (activeWindows.Count > 0 && max < 10)
			{
				Hide(activeWindows.Last());
				++max;
			}
		}

		public void Hide(WindowType windowType)
		{
			var window = activeWindows.Find(x => x.Properties.type == windowType);
			HideWindow(window);
		}

		public void Hide(string windowName)
		{
			var window = activeWindows.Find(x => x.Properties.name == windowName);
			HideWindow(window);
		}

		private void HideWindow(WindowInstance window)
		{
			if (window == null || !window.Instance)
				return;

			if (window.Instance.Status == ElementStatus.Hidden)
			{
				activeWindows.Remove(window);
				_factory.DestroyWindow(window);
			}
			else
			{
				Hide(window);
			}
		}

		private void Hide(WindowInstance window)
		{
			if (window == null || !window.Instance)
				return;

			if (waitedWindows.ContainsKey(window))
				waitedWindows.Remove(window);

			if (window.Instance.Status == ElementStatus.Hiding || window.Instance.Status == ElementStatus.Hidden)
				return;

			OnHideAction(window);

			window.Instance.Hide(() => { OnHiddenAction(window); });
		}

		private void OnHiddenAction(WindowInstance window)
		{
			TopWindow = activeWindows.Count > 0 ? (int) activeWindows.Last().Properties.type : -1;

			if (activeWindows.Count > 0 && !activeWindows.Last().Properties.IsShadowUpperHUD)
				shadow.ChangeCanvasState(true);
			else
				shadow.ChangeCanvasState(false);

			if (window.Properties.IsHasShadow && window.Properties.IsHideShadowOnEndAnimation)
			{
				var windowWithShadow = GetClosestWindowWithShadow();
				if (windowWithShadow != null)
				{
					var newIndex = windowWithShadow.Instance.transform.GetSiblingIndex();
					var shadownIndex = shadow.transform.GetSiblingIndex();
					if (shadownIndex + 1 != newIndex)
						shadow.transform.SetSiblingIndex(newIndex);
				}
				else
				{
					shadow.Hide();
				}
			}

			if (window.Properties.IsHideOtherWindows && window.Properties.IsShowHiddenWindowsOnEndAnimations)
			{
				var windowWithHide =
					activeWindows.Find(x => x.Instance.gameObject.activeSelf && x.Properties.IsHideOtherWindows);
				if (windowWithHide == null)
					SetActiveAllWindows(true);
			}

			_factory.DestroyWindow(window);
		}

		private void OnHideAction(WindowInstance window)
		{
			activeWindows.Remove(window);
			closingWindows.Add(window);
			window.Instance.OnHiddenAction += () => { ClosingComplete(window); };

			if (window.Properties.IsHideOtherWindows && !window.Properties.IsShowHiddenWindowsOnEndAnimations)
			{
				var windowWithHide =
					activeWindows.Find(x => x.Instance.gameObject.activeSelf && x.Properties.IsHideOtherWindows);
				if (windowWithHide == null)
					SetActiveAllWindows(true);
			}

			if (window.Properties.IsHasShadow && !window.Properties.IsHideShadowOnEndAnimation)
			{
				var windowWithShadow = GetClosestWindowWithShadow();
				if (windowWithShadow != null)
				{
					var newIndex = windowWithShadow.Instance.transform.GetSiblingIndex();
					var shadownIndex = shadow.transform.GetSiblingIndex();
					if (shadownIndex < newIndex)
						newIndex--;
					shadow.transform.SetSiblingIndex(newIndex);
				}
				else
				{
					shadow.Hide();
				}
			}
		}

		private void ClosingComplete(WindowInstance window)
		{
			closingWindows.Remove(window);
		}

		private WindowInstance GetClosestWindowWithShadow()
		{
			var windowsWithShadow =
				activeWindows.FindAll(x => x.Instance.gameObject.activeSelf && x.Properties.IsHasShadow);
			WindowInstance closest = null;
			foreach (var w in windowsWithShadow)
			{
				if (closest == null)
				{
					closest = w;
				}
				else
				{
					int currentSiblingIndex = closest.Instance.transform.GetSiblingIndex();
					int wSiblingIndex = w.Instance.transform.GetSiblingIndex();
					if (wSiblingIndex > currentSiblingIndex)
						closest = w;
				}
			}

			return closest;
		}

		public bool IsCanShowWindow(string windowName)
		{
			if (!_factory.GetWindow(windowName, out var window))
				return false;

			return IsCanShowWindow(window);
		}

		public bool IsCanShowWindow(WindowType windowType)
		{
			if (!_factory.GetWindow(windowType, out var window))
				return false;

			return IsCanShowWindow(window);
		}

		private bool IsCanShowWindow(WindowInstance window)
		{
			//TODO !!! Поправить  !!!
			if (_sceneLoaderManager.InProgress)
				return false;

			// нельзя показать из-за важного активного мета-действия
			//TODO ImportantAction
			// if (HasImportantAction && !window.Properties.IsShowInMetaAction)
			// 	return false;

			// нельзя показать, так как открыто окно с большим приоритетом
			if (activeWindows.Count > 0)
			{
				if (activeWindows[0].Properties.priority > window.Properties.priority)
				{
					return false;
				}
			}

			return true;
		}

		private void SetActiveAllWindows(bool value)
		{
			for (var index = activeWindows.Count - 1; index >= 0; index--)
			{
				var wp = activeWindows[index];
				//при обычном выключении отваливаются спайн анимации окон (не отрабатывает show)

				if (value)
				{
					wp.Instance.Show();
					if (wp.Properties.IsHasShadow)
					{
						shadow.Show();
						var newIndex = wp.Instance.transform.GetSiblingIndex();
						shadow.transform.SetSiblingIndex(newIndex < 0 ? 0 : newIndex);
					}

					if (wp.Properties.IsHideOtherWindows)
						break;
				}
				else
					wp.Instance.HideInstant();
			}
		}

		public bool GetWindow(WindowType windowType, out BaseWindow baseWindow)
		{
			if (_factory.GetWindow(windowType, out WindowInstance window))
			{
				if (window.Instance != null)
				{
					baseWindow = window.Instance;
					return true;
				}
			}

			baseWindow = null;
			return false;
		}

		public bool GetWindowInstance(WindowType windowType, out WindowInstance instance)
		{
			if (_factory.GetWindow(windowType, out WindowInstance windowInstance))
			{
				if (windowInstance.Instance != null)
				{
					instance = windowInstance;
					return true;
				}
			}

			instance = null;
			return false;
		}

		public bool GetWindowInstance(string windowID, out WindowInstance instance)
		{
			if (_factory.GetWindow(windowID, out WindowInstance windowInstance))
			{
				if (windowInstance.Instance != null)
				{
					instance = windowInstance;
					return true;
				}
			}

			instance = null;
			return false;
		}

		public bool GetWindow(string name, out BaseWindow baseWindow)
		{
			if (_factory.GetWindow(name, out WindowInstance windowInstance))
			{
				if (windowInstance.Instance != null)
				{
					baseWindow = windowInstance.Instance;
					return true;
				}
			}

			baseWindow = null;
			return false;
		}

		public bool IsWindowActive(WindowType windowType)
		{
			if (_factory.GetWindow(windowType, out WindowInstance window))
			{
				if (window.Instance != null && window.Instance.IsShown())
				{
					return true;
				}
			}

			return false;
		}

		public bool IsWindowInProcess()
		{
			foreach (var window in activeWindows)
			{
				if (window.Instance.Status != ElementStatus.Shown)
					return true;
			}

			return false;
		}

		public bool HasActiveWindow(WindowType type) => activeWindows.Any(x => x.Properties.type == type);

		public bool GetTopWindow(out BaseWindow window)
		{
			window = HasActiveWindows ? activeWindows[activeWindows.Count - 1].Instance : null;
			return window;
		}
	}
}