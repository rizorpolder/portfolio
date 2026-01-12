using System;
using System.Collections.Generic;
using Game.Scripts.Systems.Character.Data;
using Game.Scripts.Systems.Character.Reaction;
using UnityEngine;

namespace Game.Scripts.Systems.Character
{
	public class Character
	{
		public readonly string Name;

		public Character(string name)
		{
			Name = name;
		}

		public CharacterView View { get; private set; }

		public void SetView(CharacterView view)
		{
			View = view;
		}

		public CharacterNavigation Navigation { get; private set; }

		public void SetNavigation(CharacterNavigation navigaion)
		{
			Navigation = navigaion;
		}

		public CharacterAnimator Animator { get; private set; }

		public void SetAnimator(CharacterAnimator animator)
		{
			Animator = animator;
		}

		public NavigationAction NavigationAction { get; private set; }
		public event Action<Character> NavigationActionChanged;

		public void ReplaceNavigationAction(NavigationAction navigationAction)
		{
			NavigationAction = navigationAction;
			NavigationActionChanged?.Invoke(this);
		}

		public State State { get; private set; }
		public State LastState { get; private set; }
		public event Action<Character> StateChanged;

		public void ReplaceState(State state, State lastState)
		{
			State = state;
			LastState = lastState;
			StateChanged?.Invoke(this);
		}

		public void SetLastState()
		{
			LastState = State;
		}
		
		public CharacterPosition Position { get; private set; }
		public bool HasPosition => Position != null;
		public void ReplacePosition(Queue<Vector3> newTargets, bool newInstantly, Action<MoveCompleteStatus> newAction)
		{
			Position = new CharacterPosition()
			{
				targets = newTargets,
				instantly = newInstantly,
				action = newAction
			};
		}
		

		#region Reaction

		public ReactionState ReactionState { get; private set; }
		public event Action<Character> ReactionStateChanged;

		public void ReplaceReactionState(ReactionState state)
		{
			ReactionState = state;
			ReactionStateChanged?.Invoke(this);
			if (View)
				View.OnCharacterReactionState(ReactionState);
		}

		public Dictionary<ReactionType, List<IReaction>> AllReactions;
		public bool HasAllReactions => AllReactions != null;

		public void SetAllReactions(Dictionary<ReactionType, List<IReaction>> allreactions)
		{
			AllReactions = allreactions;
		}

		public IReaction Reaction { get; private set; }
		public bool HasReaction => Reaction != null;

		public void PlayReaction(IReaction reaction)
		{
			Reaction = reaction;
			ReplaceReactionState(ReactionState.Play);
		}

		public void ClearReaction()
		{
			Reaction = null;
			LastReaction = null;
			ReplaceReactionState(ReactionState.Idle);
		}

		public IReaction LastReaction { get; private set; }
		public bool HasLastReaction => LastReaction != null;

		public void ReplaceLastReaction(IReaction lastReaction)
		{
			LastReaction = lastReaction;
		}

		#endregion
	}
}