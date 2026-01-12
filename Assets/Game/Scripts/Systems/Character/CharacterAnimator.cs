using UnityEngine;

namespace Game.Scripts.Systems.Character
{
	public class CharacterAnimator : MonoBehaviour
	{
		public static readonly string IdleAnimation = "idle";

		private int _velocityKey;
		private int _directionXKey;
		private int _directionYKey;

		[SerializeField] Animator _animator;
		private float _defaultAnimatorSpeed = 1f;

		private void Start()
		{
			_velocityKey = Animator.StringToHash("velocity");
			_directionXKey = Animator.StringToHash("directionX");
			_directionYKey = Animator.StringToHash("directionY");
		}

		public void Init()
		{
			_defaultAnimatorSpeed = _animator.speed;
		}

		public void PlayAnimation(string characterAnimation, float transitionDuration = 0.1f)
		{
			var state = _animator.GetCurrentAnimatorStateInfo(0);
			if (!state.IsName(characterAnimation))
				_animator.CrossFadeInFixedTime(characterAnimation, transitionDuration);
		}

		public bool IsIdleAnimationPlaying()
		{
			var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			return stateInfo.IsName(IdleAnimation);
		}

		public void ToIdle()
		{
			if (!IsIdleAnimationPlaying())
			{
				if (_animator.gameObject.activeInHierarchy)
					_animator.CrossFadeInFixedTime(IdleAnimation, 0.1f);
			}
		}

		public void SetVelocity(float velocity)
		{
			_animator.SetFloat(_velocityKey, velocity, 0f, Time.deltaTime);
		}

		public void SetDirection(Vector2 direction)
		{
			var snapped = SnapToDiagonalDirection(direction);
			_animator.SetFloat(_directionXKey, snapped.x, 0f, Time.deltaTime);
			_animator.SetFloat(_directionYKey, snapped.y, 0f, Time.deltaTime);
		}

		Vector2 SnapToDiagonalDirection(Vector2 input)
		{
			if (input == Vector2.zero)
				return Vector2.zero;

			float x = Mathf.Sign(input.x);
			float y = Mathf.Sign(input.y);

			return new Vector2(x, y);
		}

		public void SetTrigger(string trigger)
		{
			_animator.SetTrigger(trigger);
		}

		public void SetSpeed(float speed)
		{
			_animator.speed = speed;
		}

		public void SetDefaultSpeed()
		{
			_animator.speed = _defaultAnimatorSpeed;
		}
	}
}