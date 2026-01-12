using AudioManager.Runtime.Core.Manager;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Initialize.Interfaces;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.Loading;
using Game.Scripts.Systems.QuestSystem;
using Game.Scripts.UI.Common;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Initialize
{
	public class TestingRoomDataLoader : MonoBehaviour, ISetup
	{
		[Inject] private SceneLoaderManager _sceneLoaderManager;
		[Inject] private GlobalUI _globalUI;
		[Inject] private SignalBus _signalBus;
		[Inject] private  ManagerAudio _managerAudio;
		private void Awake()
		{
			_signalBus.Fire(new SceneDataLoadedSignal
			{
				SetupInstance = this
			});
		}

		public UniTask Setup()
		{
			return Initialize();
		}

		private async UniTask Initialize()
		{
			_managerAudio.PlayMusic(nameof(TAudio.click), null);
			_sceneLoaderManager.Hide();
		}
	}
}