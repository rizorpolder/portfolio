using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts.Extensions;
using Game.Scripts.Systems.Character.Reaction;
using Game.Scripts.Systems.Character.Reaction.Config;
using Game.Scripts.Systems.Character.Reaction.Data;
using Game.Scripts.Systems.DialogueSystem;
using Game.Scripts.Systems.Initialize;
using Game.Scripts.Systems.Navigation;
using Game.Scripts.Systems.PlayerController;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Systems.Character
{
	public class CharacterFactory
	{
		public event Action<Character> CharacterNavigationActionChanged;

		public event Action<Character> CharacterReactionStateChanged;

		public event Action<Character> CharacterStateChanged;

		private Transform _root;

		[Inject] private IPlayerCommand _command;
		[Inject] private IDialogueData _dialogueData;
		[Inject] private NavigationPointsRepository _navigationPointsRepository;
		[Inject] private ReactionsConfig _reactionsConfig;
		[Inject] private ReactionFactory _reactionFactory;
		
		[Inject] private SceneContainerCoordinator _sceneContainerCoordinator;

		public DiContainer Container => _sceneContainerCoordinator.GetContainer();
		
		private readonly Dictionary<string, Character> _characters = new Dictionary<string, Character>();

		
		
		public void SetCharactersRoot(Transform root)
		{
			_root = root;
		}

		public Character CreateCharacter(string name)
		{
			var character = new Character(name);
			character.NavigationActionChanged += CharacterNavigationHandler;
			character.StateChanged += CharacterStateHandler;
			character.ReactionStateChanged += CharacterReactionHandler;

			_characters.Add(name, character);
			return character;
		}

		public async UniTask Load(Character entity)
		{
			if (!IsActive(entity))
				return;

			var characterName = entity.Name;

			var assetGuid = _dialogueData.DialogueDatabase.GetActorField(characterName, "AssetRefGUID").ToString();
			var assetReference = new AssetReference(assetGuid);
			var prefab = await assetReference.LoadAssetAsync<GameObject>().ToUniTask();

			var viewComponent = Container.InstantiatePrefabForComponent<CharacterView>(prefab, _root);
			viewComponent.name = entity.Name;
			viewComponent.Initialize(entity);

			entity.SetView(viewComponent);

			// var isMignon = DialogueLua.GetActorField(name, "IsMignon").asBool;
			// if(isMignon)
			// 	entity.MarkAsMignon();
			// 	
			RefreshCharacter(entity);


			// добавляем реакции
			if (_reactionsConfig.TryToGetReactions(characterName, out var allReactionsData))
			{
				var allReactions = new Dictionary<ReactionType, List<IReaction>>();
				foreach (var reactionKV in allReactionsData)
				{
					var temp = CreateReactions(reactionKV.Key, reactionKV.Value);
					allReactions.Add(reactionKV.Key, temp);
				}

				entity.SetAllReactions(allReactions);
			}
		}

		private List<IReaction> CreateReactions(ReactionType reactionType, List<ReactionData> reactionsData)
		{
			switch (reactionType)
			{
				case ReactionType.None:
					break;
				case ReactionType.Monologue:
					return GetReactions<MonologueReaction>(reactionsData);
				// case ReactionType.Movement:
				// 	return GetReactions<PathReaction>(reactionsData);
				case ReactionType.Animation:
					break;
				case ReactionType.Dialogue:
					return GetReactions<DialogueReaction>(reactionsData);
			}

			return null;
		}

		private List<IReaction> GetReactions<T>(List<ReactionData> reactionsData) where T : BaseReaction, new()
		{
			var result = new List<IReaction>();

			foreach (var data in reactionsData)
			{
				var reaction = _reactionFactory.GetReaction<T>();
				reaction.ReactionData = data;
				result.Add(reaction);
			}

			return result;
		}

		private bool IsActive(Character entity)
		{
			var actor = _dialogueData.DialogueDatabase.GetActorField(entity.Name, "IsActive").ToString();
			return bool.TryParse(actor, out var isActive) && isActive;
		}

		private void CharacterNavigationHandler(Character character)
		{
			CharacterNavigationActionChanged?.Invoke(character);
		}

		private void CharacterStateHandler(Character character)
		{
			CharacterStateChanged?.Invoke(character);
		}

		private void CharacterReactionHandler(Character character)
		{
			CharacterReactionStateChanged?.Invoke(character);
		}

		public Character GetCharacterByState(State state)
		{
			return _characters.Values.FirstOrDefault(x => x.State.Equals(state));
		}

		public Character GetPlayer()
		{
			return GetCharacterByState(State.UnderPlayerControl);
		}

		public Character GetCharacter(string actorName)
		{
			Character c = null;
			_characters.TryGetValue(actorName, out c);
			return c;
		}

		public List<Character> GetCharacters()
		{
			var list = new List<Character>();
			list.AddRange(_characters.Values);
			return list;
		}

		public void RefreshCharacter(Character character)
		{
			var characterName = character.Name;
			if (!_command.HavePlayerPositions(characterName, out Vector3 position))
			{
				var pos = _dialogueData.DialogueDatabase.GetActorField(characterName, "InitialPoint").ToString();
				position = _navigationPointsRepository.GetPoint(pos);
			}

			character.View.transform.localPosition = position;

			var stateString = _dialogueData.DialogueDatabase.GetActorField(characterName, "CharacterType").ToString();
			var characterType = Enum<CharacterType>.Parse(stateString, CharacterType.NPC);
			State state = characterType == CharacterType.Player ? State.UnderPlayerControl : State.Idle;
			character.ReplaceState(state, state);
		}

		public bool HasCharacter(string id)
		{
			return _characters.ContainsKey(id);
		}

		public void Clear()
		{
			_characters.Clear();
		}
	}
}