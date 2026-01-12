using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CooldownSystem
{
	public class CooldownService
	{
		int _timerCheckIntervalMs = 200;

		private Dictionary<string, CancellationTokenSource> _timerTasks =
			new Dictionary<string, CancellationTokenSource>();

		private CooldownManager _manager;

		public event Action<Cooldown> CooldownCompleted;

		public void RegisterTimer(Timer timer)
		{
			if (IsAlreadyRegistered(timer.Cooldown.Id))
			{
				Debug.LogError(
					$"Timer {timer.Cooldown.Id} duplicate registration!\nCompletionDate:{timer.Cooldown.CompletionDate}");
				return;
			}

			var cancellation = new CancellationTokenSource();
			_timerTasks.Add(timer.Cooldown.Id, cancellation);

			StartTimer(timer, cancellation.Token);
		}

		public void RemoveTimer(string id)
		{
			if (!_timerTasks.ContainsKey(id))
				return;

			_timerTasks[id].Cancel();
			_timerTasks[id].Dispose();
			_timerTasks.Remove(id);
		}

		public bool IsAlreadyRegistered(string id)
		{
			return _timerTasks.ContainsKey(id);
		}

		private async UniTask StartTimer(Timer timer, CancellationToken cancelToken)
		{
			timer.Cooldown.OnStarted();
			timer.Cooldown.OnUpdated();

			while (!timer.Cooldown.IsComplete)
			{
				await UniTask.Delay(_timerCheckIntervalMs, cancellationToken: cancelToken);

				if (cancelToken.IsCancellationRequested)
					return;

				timer.Cooldown.OnUpdated();
			}

			if (cancelToken.IsCancellationRequested)
				return;

			_timerTasks[timer.Cooldown.Id].Dispose();
			_timerTasks.Remove(timer.Cooldown.Id);

			try
			{
				timer.Cooldown.OnCompleted();
				CooldownCompleted?.Invoke(timer.Cooldown);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Timer {timer.Cooldown.Id} completion handler error!\n{ex.Message}");
			}
		}

		public void Wipe()
		{
			foreach (var pair in _timerTasks)
			{
				pair.Value.Cancel();
				pair.Value.Dispose();
			}

			_timerTasks.Clear();
		}
	}
}