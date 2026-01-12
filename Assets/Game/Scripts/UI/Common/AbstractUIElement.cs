using System;
using UnityEngine;

namespace Game.Scripts.UI.Common
{
	public abstract class AbstractUIElement : MonoBehaviour
	{
		public string ID = "";

		public abstract void Show(Action callback = null);
		public abstract void Hide(Action callback = null);

		protected virtual void OnShowAction()
		{
		}
		protected virtual void OnHideAction()
		{
		}

		protected virtual void OnEnableAction()
		{
		}

		protected virtual void OnDisableAction()
		{
		}

		protected virtual void OnAwakeAction()
		{
		}

		protected virtual void OnDestroyAction()
		{
		}

		protected virtual void OnResetAction()
		{
		}

		private void OnEnable()
		{
			OnEnableAction();
		}

		private void OnDisable()
		{
			OnDisableAction();
		}

		private void OnDestroy()
		{
			OnDestroyAction();
		}

		private void Awake()
		{
			OnAwakeAction();
		}

		private void Reset()
		{
			OnResetAction();

			if (ID.Length == 0)
			{
				ID = gameObject.name;
			}
		}
	}
}