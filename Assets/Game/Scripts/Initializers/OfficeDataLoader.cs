using System;
using AudioManager.Runtime.Core.Manager;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Character;
using Game.Scripts.Systems.Character.Reaction;
using Game.Scripts.Systems.DialogueSystem;
using Game.Scripts.Systems.Initialize.Interfaces;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.Input;
using Game.Scripts.Systems.Loading;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.Session;
using Game.Scripts.Systems.Statistic;
using Game.Scripts.Systems.Statistic.Data;
using Game.Scripts.UI.Common;
using UnityEngine;
using Zenject;

public class OfficeDataLoader : MonoBehaviour, ISetup
{
	[SerializeField] private Transform _charactersRoot;
	[SerializeField] private Transform _worldRoot;

	[Inject] private SceneLoaderManager _sceneLoaderManager;

	[Inject] private CharacterFactory _characterFactory;
	[Inject] private IDialogueData _dialogueData;
	[Inject] private ManagerAudio _audioManager;
	[Inject] private GlobalUI _globalUI;

	[Inject] private SignalBus _signalBus;
	[Inject] private DiContainer _container;
	[Inject] private InputHandler _inputHandler;

	[Inject] private PlayerConfig _playerConfig;
	[Inject] private IPlayerData _playerData;

	
	[Inject] private ISessionCommand _sessionCommand;
	[Inject] private ISessionData _sessionData;
	[Inject] private IStatisticCommand _statisticCommand;
	
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
		//TODO запрос на загрузку данных с сервера и обновление данных

		_worldRoot.gameObject.SetActive(true);
		UpdateCharacterData();

		await LoadCharacters();


		//_reactionController.Activate();


		_globalUI.SetActiveHud(true);
		_globalUI.HUD.Show("InitializeGame");

		OnInitComplete();
	}

	private void UpdateCharacterData()
	{
	}

	private async UniTask LoadCharacters()
	{
		_characterFactory.Clear();
		_characterFactory.SetCharactersRoot(_charactersRoot);
		var actors = _dialogueData.DialogueDatabase.GetActorsList();
		foreach (var actor in actors)
		{
			var actorName = actor.Name;
			var character = _characterFactory.GetCharacter(actorName);
			if (character != null)
			{
				_characterFactory.RefreshCharacter(character);
				continue;
			}

			if (!actor.IsActive)
				continue;

			var entity = _characterFactory.CreateCharacter(actorName);
			await _characterFactory.Load(entity);
		}
	}

	private void OnInitComplete()
	{
		_audioManager.PlayMusic(nameof(TAudio.click), null);
		_sceneLoaderManager.Hide();
		var player = _characterFactory.GetPlayer();
		_inputHandler.SetCameraPosition(player.View.transform.position);
		CheckEvents();
	}
	
	private void CheckEvents()
	{
		if (DateTime.Now.DayOfWeek is DayOfWeek.Sunday or DayOfWeek.Saturday) //TODO Network Date Time
		{
			_statisticCommand.CreateAction(StatisticActionType.HolidayGame);
		}

		if (_sessionData.SessionData.DaysFromFirstLaunch() > 14)
		{
			_statisticCommand.CreateAction(StatisticActionType.TwoWeeks);
		}

		if (_sessionData.SessionData.HoursFromLastSession() is > 24 and < 48)
		{
			_statisticCommand.CreateAction(StatisticActionType.DaysInRow);
		}
	}
}