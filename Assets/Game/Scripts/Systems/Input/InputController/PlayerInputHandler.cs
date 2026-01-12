using Game.Scripts.Systems.Character;
using Game.Scripts.Systems.Character.Reaction;
using Game.Scripts.Systems.Input.Data;
using Game.Scripts.UI.WindowsSystem;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Input
{
	public class PlayerInputHandler : MonoBehaviour
	{
		private readonly float _maxDelta = 5;

		[Inject] private CharacterFactory _characterFactory;
		[Inject] private IInputListener _inputListener;
		[Inject] private IInputData _inputData;
		[Inject] private WindowsController _windowsController;
		[Inject] private ReactionFactory _reactionFactory;
		[Inject] private InputHandler _inputHandler;

		private Character.Character _activeCharacter;

		public Character.Character ActiveCharacter
		{
			get
			{
				_activeCharacter ??= _characterFactory.GetPlayer();

				return _activeCharacter;
			}
		}

		private void Start()
		{
			_inputListener.TouchPositionChanged += TouchPositionHandler;
		}

		private void TouchPositionHandler(TouchData obj)
		{
			var touches = _inputData.GetTouches();
			if (touches.Count >= 2)
				return;

			var inputEntity = touches[0];
			if (!CanMakeReaction(inputEntity))
				return;


			if (ActiveCharacter is null)
				return;

			if (!IsTap(inputEntity))
				return;

			var pos = inputEntity.TouchPosition;
			var movePos = _inputHandler.Camera.ScreenToWorldPoint(pos);
			movePos.z = 0;

			if (_activeCharacter.Reaction is DialogueReaction dialogue)
			{
				dialogue.Stop();
			}

			var reaction = _reactionFactory.GetReaction<ClickReaction>();
			reaction.Initialize(ActiveCharacter, movePos, inputEntity.TouchTarget);
			ActiveCharacter.PlayReaction(reaction);
		}

		private bool IsTap(TouchData touch)
		{
			var startPosition = new Vector2(touch.StartTouchPosition.x, touch.StartTouchPosition.y);
			var lenght = (startPosition - touch.TouchPosition).magnitude;
			if (lenght > _maxDelta)
				return false;

			return true;
		}

		private bool CanMakeReaction(TouchData inputEntity)
		{
			if (!inputEntity.isTouchUp || !inputEntity.HasTouchTarget || !inputEntity.TouchTarget)
				return false;

			if (inputEntity.TouchTarget.layer == LayerMask.NameToLayer("UI"))
				return false;

			if (_windowsController.HasActiveOrInProcessWindow)
				return false;

			return true;
		}
	}
}