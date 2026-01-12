using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Extensions;
using Game.Scripts.Helpers;
using Game.Scripts.Systems.Navigation;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class ClickReaction : BaseReaction
	{
		private Vector3 _targetPosition;
		private GameObject _targetObject;
		private Character _activeCharacter;

		private bool _isOnlyMovement = false;
		private bool _isFinished = false;

		[Inject] private CharacterFactory _characterFactory;
		[Inject] private NavigationPointsRepository _navigationPointsRepository;

		public ClickReaction()
		{
		}

		public void Initialize(Character character, Vector3 targetPosition, GameObject targetObject)
		{
			_targetPosition = targetPosition;
			_targetObject = targetObject;
			_activeCharacter = character;
		}

		public override void Start(string target)
		{
			if (TryToDialogue())
				return;

			Action action = null;

			if (!TryIdleTarget(ref _targetPosition, ref action))
			{
			}

			if (!CharacterMove(_targetObject, _targetPosition, action))
			{
				OnFinish();
			}
		}

		protected override void OnFinish()
		{
			base.OnFinish();
			_isFinished = true;
			_activeCharacter.ReplacePosition(new Queue<Vector3>(), false, null);
			_activeCharacter.ReplaceNavigationAction(NavigationAction.Stop);
		}

		private bool TryToDialogue()
		{
			var characterPart = _targetObject.GetComponent<CharacterPart>();
			if (characterPart is null)
			{
				return false;
			}

			var charID = characterPart.CharacterName;
			var characterDialogueEntity = _characterFactory.GetCharacter(charID);
			if (characterDialogueEntity is null || !characterDialogueEntity.State.IsIdle())
			{
				return false;
			}

			var reactions = _activeCharacter.AllReactions[ReactionType.Dialogue]
				.Where(x => x.CheckConditions() && x.CheckOtherTargets(charID)).ToList();
			reactions.Shuffle();

			if (reactions.Count == 0)
			{
				return false;
			}

			_activeCharacter.PlayReaction(reactions[0]);

			return true;
		}

		private bool TryIdleTarget(ref Vector3 position, ref Action action)
		{
			// AInteractiveObject idleObject = _targetObject.GetComponent<AInteractiveObject>();
			// if (idleObject is null)
			// 	return false;
			//
			//
			// position = _navigationPointsRepository.GetPoint(idleObject.Point);
			// action += () =>
			// {
			// 	if (!_isFinished)
			// 	{
			// 		idleObject.Interact();
			// 		OnFinish();
			// 	}
			// };
			return true;
		}

		private bool CharacterMove(GameObject touchTarget, Vector3 destination, Action callback = null)
		{
			if (!_activeCharacter.View.gameObject.activeSelf)
				return false;

			var view = _activeCharacter.View as CharacterView;
			var canMove = _activeCharacter.CalculatePath(destination, out var path);

			if (canMove)
			{
				if (_isOnlyMovement)
				{
					_activeCharacter.TeleportTo(path.corners.Last(), 180, null);
				}
				else
				{
					_activeCharacter.WarpToViewPortAndMoveTo(destination, _ => callback?.Invoke());
				}
			}
			else
			{
				_activeCharacter.Navigation.Stop();
				if (Vector3.Distance(view.transform.position, destination) < _activeCharacter.Navigation.Radius)
				{
					callback?.Invoke();
				}

				return false;
			}

			return true;
		}

		public override void Stop()
		{
		}
	}
}