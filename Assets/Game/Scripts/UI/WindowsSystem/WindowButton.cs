using Game.Scripts.UI.Buttons;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.WindowsSystem
{
	public class WindowButton : BaseButton
	{
		[SerializeField] private WindowType _windowType;
		[SerializeField] private string _windowName;
		[SerializeField] private bool _show;

		[Inject] private WindowsController _controller;

		public WindowType WindowType
		{
			get { return _windowType; }
			set { _windowType = value; }
		}

		public string WindowName
		{
			get { return _windowName; }
			set { _windowName = value; }
		}

		protected override void OnClickAction()
		{
			// if (Context.IsLoading)
			// {
			// 	Log.Message($"Click on {name} prevented due to loading");
			// 	return;
			// }

			base.OnClickAction();
			WindowAction();
		}

		protected virtual void WindowAction()
		{
			if (_windowType == WindowType.Custom)
			{
				WindowByName();
			}
			else
			{
				WindowByType();
			}
		}

		protected virtual void OnShownWindowAction(BaseWindow window)
		{
		}

		protected virtual void OnHiddenWindowAction()
		{
		}

		void WindowByType()
		{
			if (_show)
			{
				_controller.Show(_windowType,
					(BaseWindow window) =>
					{
						OnShownWindowAction(window);
						window.OnHiddenAction += OnHiddenWindowAction;
					},
					WindowSource.Button);
			}
			else
				_controller.Hide(_windowType);
		}

		void WindowByName()
		{
			if (_show)
			{
				_controller.Show(WindowName,
					(BaseWindow window) =>
					{
						OnShownWindowAction(window);
						window.OnHiddenAction += OnHiddenWindowAction;
					},
					WindowSource.Button
				);
			}
			else
				_controller.Hide(WindowName);
		}
	}
}