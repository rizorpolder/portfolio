using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Systems.Initialize.Interfaces;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.Loading;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.QuestSystem;
using Game.Scripts.Systems.QuestSystem.Data;
using Game.Scripts.UI.WindowsSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts
{
	public class CreateCharacterDataLoader : MonoBehaviour, ISetup
	{
		[SerializeField] private Image _backgroundImage;

		[Inject] private WindowsController _windowsController;
		[Inject] private SceneLoaderManager _sceneLoaderManager;

		[Inject] private SignalBus _signalBus;
		[Inject] private IQuestCommand _questCommand;
		[Inject] private IPlayerCommand _playerCommand;
		[Inject] private IPlayerData _playerData;
		[Inject] private NetworkSystem _networkSystem;

		private BaseWindow _windowInstance;

		private void Awake()
		{
			_signalBus.Fire(new SceneDataLoadedSignal
			{
				SetupInstance = this
			});
		}

		public UniTask Setup()
		{
			_backgroundImage.DOColor(Color.white, 0.2f).OnComplete(ShowTutorial);
			return UniTask.CompletedTask;
		}

		private void ShowTutorial()
		{
			_sceneLoaderManager.Hide();
		
		}

		private void TutorialFinished()
		{

		}

		private void OnAskWindowClose()
		{
			_windowsController.Show(WindowType.InitializePlayer,
				(window) =>
				{
					_windowInstance = window;
					window.OnHiddenAction += OnWindowHiddenAction;
				});
		}

		private async void OnWindowHiddenAction()
		{
			_windowInstance.OnHiddenAction -= OnWindowHiddenAction;
			_windowInstance = null;
			var data = _playerData.PlayerID.Split('_', StringSplitOptions.RemoveEmptyEntries);
			await _networkSystem.Login(data[0], data[1]);

			_questCommand.ChangeQuestState("q1_1", QuestState.Active);
			_sceneLoaderManager.Load(SceneNames.Office);
		}

#if UNITY_WEBGL && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void CloseWindow();
#endif

		private void QuitAndClose()
		{
			LoadStartScene();
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
			{
				UnityEditor.EditorApplication.isPlaying = false;
			}
#else
			Application.Quit();

#endif
#if UNITY_WEBGL && !UNITY_EDITOR
			CloseWindow();
#endif
		}

		private void LoadStartScene()
		{
			_networkSystem.Disconnect();
			//_sceneLoaderManager.Load(SceneNames.Start);
		}
	}
}