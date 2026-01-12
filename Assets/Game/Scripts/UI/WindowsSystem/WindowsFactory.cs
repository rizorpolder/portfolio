using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.Scripts.UI.WindowsSystem
{
	public class WindowsFactory
	{
		private List<WindowInstance> instances = new List<WindowInstance>();

		[Inject] private DiContainer _container;

		public WindowsFactory(WindowsConfig config)
		{
			Initialize(config);
		}

		private void Initialize(WindowsConfig config)
		{
			foreach (var properties in config.Windows)
			{
				instances.Add(new WindowInstance(properties));
			}
		}

		public bool GetWindow(string name, out WindowInstance window)
		{
			window = instances.Find(x => x.Properties.name == name);
			return window != null;
		}

		public bool GetWindow(WindowType type, out WindowInstance window)
		{
			window = instances.Find(x => x.Properties.type == type);
			return window != null;
		}

		public void CreateWindow(WindowInstance window, Transform parent, Action callback)
		{
			// если есть закешированное окно
			if (window.Instance != null)
			{
				window.Instance.transform.SetParent(parent);
				callback?.Invoke();
				return;
			}

			// загружаем из ассета
			window.Properties.assetReference.InstantiateAsync(parent, false).Completed +=
				delegate(AsyncOperationHandle<GameObject> task)
				{
					window.Instance = task.Result.GetComponent<BaseWindow>();
					_container.InjectGameObjectForComponent<BaseWindow>(task.Result);
					callback?.Invoke();
				};
		}

		public void DestroyWindow(WindowInstance window)
		{
			if (!window.Properties.IsCached)
			{
				window.Destroy();
			}
		}
	}
}