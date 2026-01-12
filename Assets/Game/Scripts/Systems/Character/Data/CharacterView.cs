using Game.Scripts.Systems.Character.Reaction;
using Game.Scripts.UI.DialogueSystem;
using UnityEngine;

namespace Game.Scripts.Systems.Character
{
	public class CharacterView : MonoBehaviour
	{
		[SerializeField] private CharacterNavigation _navigation;
		[SerializeField] private CharacterAnimator _animator;
		[SerializeField] private MainBarkUI _characterBark;

		private bool _isCharacterVisible;
		public bool IsVisible => _isCharacterVisible;

		private Character _instance;

		public void Initialize(Character character) //initialize after create GO
		{
			_instance = character;
			_instance.SetNavigation(_navigation);
			_instance.SetAnimator(_animator);
			_navigation.AddAnimator(_animator);
			_navigation.Init();
			_animator.Init();
		}

		public MainBarkUI GetBarkUI() => _characterBark;

		public void OnCharacterReactionState(ReactionState value)
		{
			if (value == ReactionState.Terminated)
			{
				//TODO OnReaction Terminated
			}
		}
	}
}