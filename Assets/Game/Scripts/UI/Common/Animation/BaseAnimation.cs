using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.UI.Common.Animation
{
	public abstract class BaseAnimation : MonoBehaviour
	{
		public delegate void PostAnimationAction();

		public virtual bool IsInitialized => true;

		private void Awake()
		{
			OnAwake();
		}

		private void Start()
		{
			OnStart();
		}

		protected abstract void OnAwake();
		protected abstract void OnStart();
		public abstract UniTask Show(PostAnimationAction action = null);
		public abstract UniTask Hide(PostAnimationAction action = null);

		public abstract UniTask Play(string name, PostAnimationAction action = null);
	}
}