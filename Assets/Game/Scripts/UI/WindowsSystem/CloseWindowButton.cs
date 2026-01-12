using AudioManager.Runtime.Core.Manager;
using Game.Scripts.UI.Common;
using Plugins.AudioManager.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.WindowsSystem
{
	[RequireComponent(typeof(Button))]
	public class CloseWindowButton : MonoBehaviour
	{
		[SerializeField] protected Button _button;
		[SerializeField] private BaseWindow _window;

		[Inject] private ManagerAudio _managerAudio;

		public void Awake()
		{
			_button.onClick.AddListener(CloseWindow);
		}

		protected void CloseWindow()
		{
			if (_window == null || _window.Status != ElementStatus.Shown)
				return;

			_managerAudio.PlayAudioClip(TAudio.click);

			_window.Close();
		}

		protected void OnValidate()
		{
			if (_window == null)
			{
				_window = gameObject.GetComponentInParent<BaseWindow>(true);
			}

			if (_button == null)
			{
				_button = GetComponent<Button>();
			}
		}
	}
}