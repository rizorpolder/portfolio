using System;
using Game.Scripts.UI.Common.Animation;
using UnityEngine;

namespace Game.Scripts.UI.Common
{
	public enum ElementStatus
	{
		Hidden,
		Showing,
		Shown,
		Hiding,

		IsShown = Shown | Showing,
		IsHidden = Hidden | Shown,
	}

	public class BasePanel : AbstractUIElement
	{
		public event Action OnShownAction;
		public event Action OnHiddenAction;

		[SerializeField] protected BaseAnimation _animationElement;
		[SerializeField] protected BaseAnimation[] additionalAnimationElements;

		[SerializeField] protected ElementStatus _status = ElementStatus.Hidden;
		[SerializeField] private bool _disableAfterHide = true;

		public ElementStatus Status
		{
			get { return _status; }
		}

		public bool IsHidden() => Status == ElementStatus.Hidden || Status == ElementStatus.Hiding;
		public bool IsShown() => Status == ElementStatus.Shown || Status == ElementStatus.Showing;

		public override void Show(Action callback = null)
		{
			if (IsShown())
				return;

			gameObject.SetActive(true);

			if (HasAnimation())
			{
				_status = ElementStatus.Showing;

				foreach (var anim in additionalAnimationElements)
				{
					anim.Show();
				}

				_animationElement.Show(() =>
				{
					if (_status == ElementStatus.Showing)
					{
						OnShownAction?.Invoke();
						callback?.Invoke();
						_status = ElementStatus.Shown;
					}
				});
			}
			else
			{
				OnShownAction?.Invoke();
				callback?.Invoke();
				_status = ElementStatus.Shown;
			}

			OnShowAction();
		}

		public override void Hide(Action callback = null)
		{
			if (IsHidden())
				return;

			_status = ElementStatus.Hiding;

			if (HasAnimation())
			{
				foreach (var anim in additionalAnimationElements)
				{
					anim.Hide();
				}

				_animationElement.Hide(() =>
				{
					if (_status == ElementStatus.Hiding)
					{
						callback?.Invoke();
						HidingComplete();
					}
				});
			}
			else
			{
				callback?.Invoke();
				HidingComplete();
			}

			OnHideAction();
		}

		public virtual void HideInstant(Action callback = null)
		{
			if (IsHidden())
				return;

			callback?.Invoke();
			HidingComplete();

			OnHideAction();
		}

		private bool HasAnimation()
		{
			if (!_animationElement)
				_animationElement = GetComponent<BaseAnimation>();

			return _animationElement;
		}

		protected virtual void HidingComplete()
		{
			_status = ElementStatus.Hidden;
			if (_disableAfterHide && gameObject != null)
				gameObject.SetActive(false);
			OnHiddenAction?.Invoke();
			
			//OnHiddenAction = null; Пока окна не дестроятся - не будем
		}

		public void Play(string trigger)
		{
			if (_animationElement != null)
				_animationElement.Play(trigger);
		}

		protected override void OnResetAction()
		{
			if (_animationElement == null)
				_animationElement = GetComponent<BaseAnimation>();
		}

		public void ResetHiddenAction()
		{
			OnHiddenAction = null;
		}
	}
}