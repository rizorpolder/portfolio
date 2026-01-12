using System;
using AudioManager.Runtime.Core.Manager;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.Texts;
using Plugins.AudioManager.Runtime.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.DialogueSystem
{
	[RequireComponent(typeof(Button))]
	public class ResponseButton : MonoBehaviour
	{
		public Action<Response> OnSelectResponse;

		[SerializeField] private Button _button;
		[SerializeField] private TextMeshProUGUI _text;
		private Response _response = null;

		[Inject] private TextsValidationSystem _textsValidationSystem;
		[Inject] private ManagerAudio _managerAudio;
		private void Start()
		{
			_button.onClick.AddListener(OnResponseSelected);
		}

		public void Initialize(Response response)
		{
			_response = response;
			_text.text = _textsValidationSystem.GetLocalizedString(_response.FormattedText);
		}

		private void OnResponseSelected()
		{
			_managerAudio.PlayAudioClip(TAudio.click);

			OnSelectResponse?.Invoke(_response);
		}
	}
}