using System;
using AudioManager.Runtime.Core.Manager;
using Cysharp.Threading.Tasks;
using Game.Scripts.Common.Pool;
using Game.Scripts.Extensions;
using Game.Scripts.Systems.DialogueSystem.Data;
using Game.Scripts.Systems.DialogueSystem.Events;
using Plugins.AudioManager.Runtime.Core;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.DialogueSystem
{
	public class DialogueUI : MonoBehaviour
	{
		public event EventHandler<SelectedResponseEventArgs> SelectedResponseHandler;
		public event Action OnContinueEvent;

		[SerializeField] private Button _continueButton;
		[SerializeField] private DialoguePanel _panel;

		[SerializeField] private ResponseButton _responseButtonPrefab;
		[SerializeField] private Transform _buttonsContainer;

		[Inject] private ObjectsPoolFactory _poolFactory;
		[Inject] private ManagerAudio _managerAudio;
		private ObjectsPool<ResponseButton> _pool;

		private bool _isOpen = false;
		private Subtitle _lastSubtitle;

		private void Awake()
		{
			if (_panel != null)
			{
				_panel.Init();
			}

			_continueButton.onClick.AddListener(OnContinueButtonHandler);
		}

		private void Start()
		{
			_pool = _poolFactory.CreatePool(_responseButtonPrefab, _buttonsContainer);
		}

		private void OnContinueButtonHandler()
		{
			if (!_isOpen)
				return;

			if (_panel.TrySkipTypewriter())
				return;

			_managerAudio.PlayAudioClip(TAudio.click);
			OnContinueEvent?.Invoke();
		}

		public void Open()
		{
			_isOpen = true;
			_panel.Open();
			_continueButton.gameObject.SetActive(true);
		}

		public void Close()
		{
			_isOpen = false;
			_panel.Close();
			_continueButton.gameObject.SetActive(false);
		}

		public void ShowSubtitle(Subtitle subtitle)
		{
			_lastSubtitle = subtitle;
			_panel.Show(_lastSubtitle);
			if (subtitle.AutoTransition)
			{
				OnContinueButtonHandler();
			}
		}

		public void HideSubtitle()
		{
			_panel.Hide();
		}

		public async UniTaskVoid ShowResponses(Subtitle subtitle, Response[] responses, float timeout = 0)
		{
			await UniTask.WaitUntil(() => !_panel.IsTyping);
			_continueButton.gameObject.SetActive(false);
			responses.Shuffle();
			foreach (Response response in responses)
			{
				var button = _pool.GetItem();
				button.Initialize(response);
				button.OnSelectResponse += OnResponseSelected;
			}
		}

		private void OnResponseSelected(Response obj)
		{
			if (_panel.TrySkipTypewriter())
			{
				return;
			}

			SelectedResponseHandler?.Invoke(this, new SelectedResponseEventArgs(obj));
		}

		public void HideResponses()
		{
			foreach (var activeItem in _pool.GetActiveItems())
			{
				activeItem.OnSelectResponse -= OnResponseSelected;
			}

			_pool.ResetPool();
			_continueButton.gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			_continueButton.onClick.RemoveListener(OnContinueButtonHandler);
		}
	}
}