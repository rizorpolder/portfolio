using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Scripts.UI.Common.Animation
{
	[RequireComponent(typeof(Animator), typeof(PlayableDirector))]
	public class PlayableAnimation : BaseAnimation
	{
		[SerializeField] private PlayableDirector Director;
		[SerializeField] private PlayableAsset ShowPlayable;
		[SerializeField] private PlayableAsset HidePlayable;

		private CancellationTokenSource _showCancellation;
		private CancellationTokenSource _hideCancellation;

		protected override void OnAwake()
		{
		}

		protected override void OnStart()
		{
		}

		public override UniTask Show(PostAnimationAction action = null)
		{
			CancelHide();
			_showCancellation = new CancellationTokenSource();
			var observable = Director.PlayAsObservable(ShowPlayable)
				.AttachExternalCancellation(_showCancellation.Token);
			observable.ContinueWith(() =>
			{
				//Sometimes null, unexpected
				if (_showCancellation != null)
				{
					if (!_showCancellation.IsCancellationRequested)
						action?.Invoke();
					_showCancellation.Dispose();
					_showCancellation = null;
				}
			}).Forget();

			return observable;
		}

		public override UniTask Hide(PostAnimationAction action = null)
		{
			CancelShow();
			_hideCancellation = new CancellationTokenSource();
			var observable = Director.PlayAsObservable(HidePlayable)
				.AttachExternalCancellation(_hideCancellation.Token);
			observable.ContinueWith(() =>
			{
				//Sometimes null, unexpected
				if (_hideCancellation != null)
				{
					if (!_hideCancellation.IsCancellationRequested)
						action?.Invoke();
					_hideCancellation.Dispose();
					_hideCancellation = null;
				}
			}).Forget();

			return observable;
		}

		public override UniTask Play(string name, PostAnimationAction action = null)
		{
			return UniTask.CompletedTask;
		}

		private void CancelHide()
		{
			if (_hideCancellation != null)
			{
				_hideCancellation.Cancel();
			}
		}

		private void CancelShow()
		{
			if (_showCancellation != null)
			{
				_showCancellation.Cancel();
			}
		}
	}
}