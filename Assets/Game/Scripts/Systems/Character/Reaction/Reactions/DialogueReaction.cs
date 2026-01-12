using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Helpers;
using Game.Scripts.Systems.Actions;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.Character.Reaction.Data;
using Game.Scripts.Systems.DialogueSystem;
using Game.Scripts.Systems.QuestSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class DialogueReaction : BaseReaction
	{
		private Character _speaker;
		private Character _listener;
		private Vector3 _targetPosition;

		[Inject] private CharacterFactory _characterFactory;
		[Inject] private IQuestCommand _questCommand;
		[Inject] private CustomActionsFactory _customActionsFactory;
		[Inject] private ReactionFactory _reactionFactory;

		public override bool CheckConditions()
		{
			base.CheckConditions();

			if (ReactionData is not DialogueReactionData dialogueReactionData)
				return false;


			if (dialogueReactionData.conditions.QuestConditions.Any(condition =>
				    !_questCommand.CheckQuestCondition(condition)))
			{
				return false;
			}

			if (!_characterFactory.HasCharacter(dialogueReactionData.dialogueListener))
				return false;

			var entity = _characterFactory.GetCharacter(dialogueReactionData.dialogueListener);
			return entity.View && entity.View.gameObject.activeSelf && entity.State.IsIdle();
		}

		public override bool CheckOtherTargets(string target2)
		{
			var dialogueReactionData = ReactionData as DialogueReactionData;
			return dialogueReactionData.dialogueListener.Equals(target2);
		}

		public override void Start(string target)
		{
			_speaker = _characterFactory.GetCharacter(target);
			this._target = target;

			var dialogueReactionData = ReactionData as DialogueReactionData;
			if (_characterFactory.HasCharacter(dialogueReactionData.dialogueListener))
			{
				_listener = _characterFactory.GetCharacter(dialogueReactionData.dialogueListener);
				if (TryToStartDialogue())
				{
					return;
				}
			}

			OnFinish();
		}

		private bool TryToStartDialogue()
		{
			if (!_listener.View || !_listener.View.gameObject.activeSelf)
				return false;

			_targetPosition = _listener.View.transform.position;

			if (!NavigationHelper.IsReachablePathForAgent(_speaker.Navigation, _targetPosition, out var path))
			{
				return false;
			}

			// если нельзя дойти (перс в заблокированной комнате)
			var nextPosition = path.corners.Last();
			if ((nextPosition - _targetPosition).magnitude > 1f)
				return false;

			_targetPosition = path.corners.Last();

			if (!CharacterMove())
			{
				return false;
			}

			// ставим слушателя в ожидание, чтобы у него не запустились другие реакции в idle-системе
			var waitReaction = _reactionFactory.GetReaction<WaitDialogueReaction>();
			waitReaction.SetSpeadeker(_target);
			_listener.PlayReaction(waitReaction);

			return true;
		}

		private void StartDialogue()
		{
			base.Start(_target);

			var action = _customActionsFactory.Create(reactionData.Action.ActionData);
			action.OnActionExecuted += OnDialogueFinish;
			action.Execute();
		}

		private bool CharacterMove()
		{
			var canMove = _speaker.CalculatePath(_targetPosition, out _);

			if (canMove)
			{
				_speaker.CreateMovement(_targetPosition)
					.OnMoveComplete(_ => StartDialogue())
					//.WarpToViewport()
					.WithStopDistance(0.7f)
					.Move();
			}

			return canMove;
		}

		public override void Stop()
		{
			OnFinish();
		}

		private void OnDialogueFinish(IActionExecutable action)
		{
			action.OnActionExecuted -= OnDialogueFinish;
			OnFinish();
		}

		protected override void OnFinish()
		{
			base.OnFinish();

			_listener?.ReplaceReactionState(ReactionState.Terminated);

			if (_speaker == null)
				return;

			_speaker.ReplacePosition(new Queue<Vector3>(), false, null);
			_speaker.ReplaceNavigationAction(NavigationAction.Stop);
		}
	}
}