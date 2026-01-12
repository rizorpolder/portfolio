using System;
using UnityEngine;

namespace Game.Scripts.UI.Common.Animation
{
	public class StateMachineBehaviourTrigger : StateMachineBehaviour
	{
		public string StateName;

		public class OnStateInfo
		{
			public Animator Animator { get; private set; }
			public AnimatorStateInfo StateInfo { get; private set; }
			public int LayerIndex { get; private set; }

			public OnStateInfo(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
			{
				Animator = animator;
				StateInfo = stateInfo;
				LayerIndex = layerIndex;
			}
		}

		public class OnStateMachineInfo
		{
			public Animator Animator { get; private set; }
			public int StateMachinePathHash { get; private set; }

			public OnStateMachineInfo(Animator animator, int stateMachinePathHash)
			{
				Animator = animator;
				StateMachinePathHash = stateMachinePathHash;
			}
		}

		// OnStateExit

		public event Action<OnStateInfo> onStateExit;

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateExit != null) onStateExit.Invoke(new OnStateInfo(animator, stateInfo, layerIndex));
		}

		// OnStateEnter

		public event Action<OnStateInfo> onStateEnter;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateEnter?.Invoke(new OnStateInfo(animator, stateInfo, layerIndex));
		}

		// OnStateIK

		public event Action<OnStateInfo> onStateIK;

		public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateIK != null) onStateIK.Invoke(new OnStateInfo(animator, stateInfo, layerIndex));
		}

		// Does not implments OnStateMove.
		// ObservableStateMachine Trigger makes stop animating.
		// By defining OnAnimatorMove, you are signifying that you want to intercept the movement of the root object and apply it yourself.
		// http://fogbugz.unity3d.com/default.asp?700990_9jqaim4ev33i8e9h

		//// OnStateMove

		//Subject<OnStateInfo> onStateMove;

		//public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    if (onStateMove != null) onStateMove.OnNext(new OnStateInfo(animator, stateInfo, layerIndex));
		//}

		//public IObservable<OnStateInfo> OnStateMoveAsObservable()
		//{
		//    return onStateMove ?? (onStateMove = new Subject<OnStateInfo>());
		//}

		// OnStateUpdate

		public event Action<OnStateInfo> onStateUpdate;

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (onStateUpdate != null) onStateUpdate.Invoke(new OnStateInfo(animator, stateInfo, layerIndex));
		}

		// OnStateMachineEnter

		public event Action<OnStateMachineInfo> onStateMachineEnter;

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			if (onStateMachineEnter != null)
				onStateMachineEnter.Invoke(new OnStateMachineInfo(animator, stateMachinePathHash));
		}

		// OnStateMachineExit

		public event Action<OnStateMachineInfo> onStateMachineExit;

		public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
		{
			if (onStateMachineExit != null)
				onStateMachineExit.Invoke(new OnStateMachineInfo(animator, stateMachinePathHash));
		}

		public void Reset()
		{
			onStateEnter = null;
			onStateUpdate = null;
			onStateExit = null;
			onStateIK = null;
			onStateMachineEnter = null;
			onStateMachineExit = null;
		}
	}
}