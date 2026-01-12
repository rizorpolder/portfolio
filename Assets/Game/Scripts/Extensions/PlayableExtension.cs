using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Scripts.Extensions
{
	public static class PlayableExtension
	{
		public static UniTask AsObservable(this PlayableDirector director)
		{
			return PlayAsObservable(director, director.playableAsset);
		}

		public static async UniTask PlayAsObservable(this PlayableDirector director, PlayableAsset playableAsset)
		{
			if (director == null)
			{
				Debug.LogAssertionFormat("PlayableDirector is null");
				return;
			}

			if (playableAsset == null)
			{
				Debug.LogAssertionFormat("PlayableAsset is null");
				return;
			}

			director.time = 0;
			director.initialTime = 0;

			var completionSource = new UniTaskCompletionSource();

			// Обработчик события остановки PlayableDirector
			void OnDirectorStopped(PlayableDirector d)
			{
				completionSource.TrySetResult();
			}

			try
			{
				director.stopped += OnDirectorStopped;

				// Запуск PlayableDirector с PlayableAsset
				director.Play(playableAsset, DirectorWrapMode.Hold);
				director.Evaluate();

				// Ожидание завершения проигрывания или окончания по времени
				while (director.time < director.duration && director.state == PlayState.Playing)
				{
					await UniTask.Yield(PlayerLoopTiming.Update);
					if (!director)
					{
						completionSource.TrySetCanceled();
						return;
					}
				}

				if (director.state == PlayState.Playing)
				{
					director.Stop();
				}

				await completionSource.Task; // Ожидание вызова OnDirectorStopped или завершения проигрывания
			}
			finally
			{
				// Убираем подписку на событие
				director.stopped -= OnDirectorStopped;
			}
		}
	}
}