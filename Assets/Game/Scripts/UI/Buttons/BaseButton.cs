using System;
using AudioManager.Runtime.Core.Manager;
using Game.Scripts.UI.Common;
using Plugins.AudioManager.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Buttons
{
	[RequireComponent(typeof(Button))]
	public class BaseButton : AbstractUIElement
	{
		[SerializeField] protected Button _button;

		[SerializeField] protected TAudio clickType = TAudio.click;
		
		[Inject] private ManagerAudio _managerAudio;
		
		public Button ButtonInstance
		{
			get { return _button; }
		}

		public event Action Clicked;

		public void Click()
		{
			OnClickAction();
		}

		public override void Hide(Action callback = null)
		{
			gameObject.SetActive(false);
		}

		public override void Show(Action callback = null)
		{
			OnShowAction();

			gameObject.SetActive(true);
		}

		protected override void OnAwakeAction()
		{
			base.OnAwakeAction();
			_button.onClick.AddListener(OnClick);
		}

		private void OnClick()
		{
			OnClickAction();
		}

		protected virtual void OnClickAction()
		{
			_managerAudio.PlayAudioClip(clickType);
			Clicked?.Invoke();
		}
	}
}