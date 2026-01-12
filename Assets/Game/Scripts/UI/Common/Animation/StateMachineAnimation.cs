using Cysharp.Threading.Tasks;
using Game.Scripts.Extensions;
using UnityEngine;

namespace Game.Scripts.UI.Common.Animation
{
	[RequireComponent(typeof(Animator))]
	public class StateMachineAnimation : BaseAnimation
	{
		private StateMachineBehaviourTrigger[] triggers;
		private const string AnimatorTriggerShow = "Show";
		private const string AnimatorTriggerHide = "Hide";
		private const string AnimatorStateShown = "Shown";
		private const string AnimatorStateHidden = "Hidden";
		private const string AnimatorBoolInstantShow = "InstantShow";

		[SerializeField] private Animator _animator;

		private bool _isInitialized;
		public override bool IsInitialized => _isInitialized;

		protected override void OnAwake()
		{
			Initialize();
		}

		protected override void OnStart()
		{
		}

		private void Initialize()
		{
			triggers ??= _animator.GetBehaviours<StateMachineBehaviourTrigger>();
			_isInitialized = true;
		}

		private void OnEnable()
		{
			triggers = _animator.GetBehaviours<StateMachineBehaviourTrigger>();
		}

		public override UniTask Show(PostAnimationAction action = null)
		{
			if (GetTrigger(AnimatorStateShown, out var trigger))
			{
				var animationState = _animator.GetCurrentAnimatorStateInfo(0);
				if (!animationState.IsName(AnimatorStateShown))
				{
					trigger.onStateEnter += (info) =>
					{
						Dispose();
						action?.Invoke();
					};
				}
				else
				{
					action?.Invoke();
				}
			}

			_animator.ResetTriggers();
			_animator.SetTrigger(AnimatorTriggerShow);

			return UniTask.CompletedTask;
		}

		public override UniTask Hide(PostAnimationAction action = null)
		{
			if (GetTrigger(AnimatorStateHidden, out var trigger))
			{
				trigger.onStateEnter += (info) =>
				{
					Dispose();
					action?.Invoke();
				};
			}

			if (_animator.gameObject.activeInHierarchy)
			{
				_animator.ResetTriggers();
				_animator.SetTrigger(AnimatorTriggerHide);
			}

			_animator.SetBool(AnimatorBoolInstantShow, false);
			return UniTask.CompletedTask;
		}

		public override UniTask Play(string name, PostAnimationAction action = null)
		{
			_animator.ResetTriggers();
			_animator.SetTrigger(name);

			return UniTask.CompletedTask;
		}

		public bool GetTrigger(string triggerName, out StateMachineBehaviourTrigger result)
		{
			result = null;
			if (!_isInitialized)
				return false;

			if (triggers == null)
			{
				Debug.LogError($"No {nameof(StateMachineBehaviourTrigger)} {triggerName} found in {name}!", gameObject);
				return false;
			}

			if (!string.IsNullOrEmpty(triggerName))
			{
				foreach (StateMachineBehaviourTrigger trigger in triggers)
				{
					if (trigger.StateName == triggerName)
					{
						result = trigger;
						return true;
					}
				}
			}

			return false;
		}

		private void OnDestroy()
		{
			Dispose();
		}

		private void Dispose()
		{
			if (triggers != null)
			{
				foreach (var trigger in triggers)
					trigger.Reset();
			}
		}

		private void Reset()
		{
			if (_animator == null)
				_animator = GetComponent<Animator>();
		}
	}
}