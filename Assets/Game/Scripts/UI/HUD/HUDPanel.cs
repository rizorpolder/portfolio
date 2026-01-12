using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.UI.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
	public class HUDPanel : BasePanel
	{
		[SerializeField] private Button[] _buttons;
		private List<Button> _runtimeButtons = new List<Button>();

		/// <summary>
		/// Дочерние панели, которые могут самостоятельно включаться при необходимости
		/// </summary>
		[SerializeField] protected HUDChildPanel[] _childs;

		/// <summary>
		/// Задержка на показ
		/// </summary>
		[SerializeField] private int _delayShow = 200;

		private bool _isCanceledShow = false;
		private HashSet<string> lockList = new HashSet<string>();

		protected override void OnAwakeAction()
		{
			base.OnAwakeAction();
			_status = ElementStatus.Shown;
		}

		public void AddLock(string reason)
		{
			lockList.Add(reason);
			SetButtonsInteractable(false);

			Hide();

			foreach (var child in _childs)
				child.Hide();

			_isCanceledShow = true;
		}

		public bool RemoveLock(string reason)
		{
			lockList.Remove(reason);

			if (lockList.Count == 0)
			{
				_isCanceledShow = false;
				Show(reason);
			}

			return lockList.Count > 0;
		}

		public bool HasLock(string reason)
		{
			return lockList.Contains(reason);
		}

		public bool HasLock()
		{
			return lockList.Count > 0;
		}

		private async void Show(string reason)
		{
			if (_delayShow > 0)
				await UniTask.Delay(_delayShow);

			if (_animationElement && !_animationElement.IsInitialized)
				await UniTask.WaitUntil(() => _animationElement.IsInitialized);

			if (_isCanceledShow)
				return;

			_status = ElementStatus.Hidden;
			Show(() => { SetButtonsInteractable(true); });

			foreach (var child in _childs)
			{
				if (child.gameObject.activeSelf && child.IsActiveOnScene() && !child.IsAlwaysHidden)
					child.Show();
			}
		}

		public void Reset()
		{
			lockList.Clear();
		}

		private void SetButtonsInteractable(bool interactable)
		{
			foreach (var button in _buttons)
			{
				if (button != null)
					button.interactable = interactable;
			}

			foreach (var button in _runtimeButtons)
			{
				if (button != null)
					button.interactable = interactable;
			}
		}

		public void AddButton(Button button)
		{
			_runtimeButtons.Add(button);
		}

		public void RemoveButton(Button button)
		{
			_runtimeButtons.Remove(button);
		}
	}
}