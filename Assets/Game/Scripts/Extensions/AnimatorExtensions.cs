using UnityEngine;

namespace Game.Scripts.Extensions
{
	public static class AnimatorExtensions
	{
		public static void ResetTriggers(this Animator animator)
		{
			if (!animator.gameObject.activeInHierarchy)
				return;

			foreach (var param in animator.parameters)
			{
				if (param.type == AnimatorControllerParameterType.Trigger)
				{
					animator.ResetTrigger(param.name);
				}
			}
		}
	}
}