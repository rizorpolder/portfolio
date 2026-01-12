using Cysharp.Threading.Tasks;
using AudioManager.Runtime.Core.Manager;
using AudioManager.Runtime.Extensions;
using Game.Scripts.Systems.Loading;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.SaveSystem;
using Game.Scripts.Systems.Session;
using Game.Scripts.Systems.Statistic;
using Plugins.AudioManager.Runtime.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Systems.Initialize
{
	public class StartDataLoader : MonoBehaviour
	{
		public static StartDataLoader Instance;

		[SerializeField] private Button _loadButton;

		[Inject] private SceneLoaderManager _sceneLoaderManager;
		[Inject] private IPlayerData _playerData;

		[Inject] private ISessionCommand _sessionCommand;
		[Inject] private ISessionData _sessionData;
		[Inject] private IStatisticCommand _statisticCommand;
		[Inject] private ISaveSystemCommand _saveSystemCommand;

		[Inject] private ManagerAudio _managerAudio;
		[Inject] private NetworkSystem _networkSystem;

		private bool IsLoadingComplete = false;

		private void Awake()
		{
			if (Instance)
			{
				Debug.LogError($"{name} duplication");
				return;
			}

			Instance = this;
		}

		private void Start()
		{
			_sceneLoaderManager.Hide();
			_loadButton.onClick.AddListener(OnLoadButtonClickHandler);
			Load();
		}

		private void Load()
		{
			TempFunc().Forget();
		}

		private async UniTaskVoid TempFunc()
		{
			await UniTask.WaitForSeconds(1);
			IsLoadingComplete = true;
			_loadButton.gameObject.SetActive(true);
		}

//Аддитивно загрузить следующую сцену. Как только всё готово - выгрузить сцену start

		public async UniTask LoadingComplete()
		{
			await UniTask.WaitUntil(() => IsLoadingComplete);
		}

		private void OnLoadButtonClickHandler()
		{
			_managerAudio.PlayAudioClip(TAudio.click);
			LoadCurrentScene().Forget();
			_loadButton.interactable = false;
		}

		private async UniTaskVoid LoadCurrentScene()
		{
			var sceneInstance = _networkSystem.IsAuthorized || !_playerData.PlayerID.IsNullOrEmpty()
				? await Addressables.LoadSceneAsync("Office", LoadSceneMode.Additive, false)
				: await Addressables.LoadSceneAsync("CreateCharacter", LoadSceneMode.Additive, false);

			await sceneInstance.ActivateAsync();

			_sessionCommand.ReplaceSessionData(_sessionData.SessionData.TrackSession());
			_saveSystemCommand.Save(SaveDataType.Session).Forget();

			_statisticCommand.StartService();

			_sceneLoaderManager.FinishLoading();
		}

		private void OnDestroy()
		{
			Instance = null;
			_loadButton.onClick.RemoveAllListeners();
		}
	}
}