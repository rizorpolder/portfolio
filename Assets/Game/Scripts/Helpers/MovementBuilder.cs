using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.Character;
using UnityEngine;

namespace Game.Scripts.Helpers
{
	public class MovementBuilder
	{
		private readonly Character _character;
		private readonly Queue<Vector3> _movementPoints;
		private readonly Vector3 _destination;

		private Action<MoveCompleteStatus> _onMoveComplete;

		private bool _needWarpToViewport;
		private bool _cinematic;
		private bool _instantly;

		private bool _instantlyMoveIfPathInvalid;

		public MovementBuilder(Character character, Queue<Vector3> movementPoints)
		{
			_character = character;
			_movementPoints = movementPoints;
			_destination = movementPoints.Last();
		}

		public MovementBuilder Instantly(bool instantly = true)
		{
			_instantly = instantly;
			return this;
		}

		public MovementBuilder OnMoveComplete(Action<MoveCompleteStatus> onMoveComplete)
		{
			_onMoveComplete = onMoveComplete;
			return this;
		}

		public MovementBuilder WarpToViewport(bool needWarpToViewport = true, bool cinematic = false)
		{
			_needWarpToViewport = needWarpToViewport;
			_cinematic = cinematic;
			return this;
		}

		public MovementBuilder WithStopDistance(float stopDistance)
		{
			_character.Navigation.SetAgentStoppingDistance(stopDistance);
			return this;
		}

		private bool WarpToViewport(Action<MoveCompleteStatus> callback)
		{
			if (_character.View && _character.View is CharacterView)
			{
				if (_movementPoints.Count == 0 || _movementPoints.Count > 1)
					return false;

				if (_character.CalculatePath(_destination, out var path))
				{
					if (_character.CanWarpToViewport(path, _cinematic, out var borderPoint))
					{
						_character.CreateMovement(borderPoint)
							.Instantly()
							.OnMoveComplete(callback)
							.Move();
						return true;
					}
				}
			}

			return false;
		}

		public void Move()
		{
			if (_needWarpToViewport)
			{
				if (!WarpToViewport(_ => NavigateCharacter()))
				{
					NavigateCharacter();
				}

				return;
			}

			NavigateCharacter();
		}

		private void OnMoveComplete(MoveCompleteStatus status)
		{
			_onMoveComplete?.Invoke(status);
		}

		private void NavigateCharacter()
		{
			if (_instantlyMoveIfPathInvalid)
			{
				if (!_character.CalculatePath(_destination, out var path))
				{
					_instantly = true;
				}
			}

			_character.ReplacePosition(_movementPoints, _instantly, OnMoveComplete);
			_character.ReplaceNavigationAction(NavigationAction.Move);
		}
	}
}