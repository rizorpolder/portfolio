using System;
using Game.Scripts.Helpers;

namespace CooldownSystem
{
	public class Cooldown
	{
		public string Id { get; }

		/// <summary>
		/// Время старта (необязательный параметр)
		/// </summary>
		public DateTime StartDate { get; set; }

		private DateTime _completionDate;

		public DateTime CompletionDate
		{
			get => _completionDate;
			set { _completionDate = value; }
		}

		public event Action Started;
		public event Action<Cooldown> Updated;
		public event Action<Cooldown> Completed;

		/// <summary>
		/// Длительность в часах (необязательный параметр)
		/// </summary>
		public double Duration;

		/// <summary>
		/// Заморожен ли таймер
		/// </summary>
		public bool IsFreeze { get; private set; }

		/// <summary>
		/// Сколько времени необходимо таймеру для зваершения после разморозки
		/// </summary>
		public TimeSpan FreezeTime { get; private set; }

		public TimeSpan TimeLeft => CompletionDate.Subtract(DateTime.Now); //TODO ServerTime

		public TimeSpan TimeAfterCompletion => DateTime.Now.Subtract(CompletionDate); //TODO ServerTime

		public bool IsComplete => !IsFreeze && DateTime.Now > CompletionDate; //TODO ServerTime

		public override string ToString()
		{
			return dateTimeString;
		}

		private string dateTimeString;

		public Cooldown(string id, long dateData)
		{
			Id = id;
			CompletionDate = DateTime.FromBinary(dateData);
		}

		public Cooldown(string id, DateTime completionDate)
		{
			Id = id;
			CompletionDate = completionDate;
		}

		public void Freeze(TimeSpan timeSpan)
		{
			IsFreeze = true;
			FreezeTime = timeSpan;
		}

		/// <summary>
		/// Замораживает текущий таймер
		/// </summary>
		public void Freeze()
		{
			IsFreeze = true;

			// Фиксируем сколько нужно время для завершения таймера
			// Необходимо для воостановления
			FreezeTime = TimeLeft;
		}

		public void DeFreeze()
		{
			IsFreeze = false;
			CompletionDate = DateTime.Now.Add(FreezeTime); //TODO ServerTime
		}

		public void OnStarted()
		{
			Started?.Invoke();
		}

		public void OnUpdated()
		{
			if (IsFreeze)
				return;

			var passedTime = CompletionDate.Subtract(DateTime.Now); //TODO ServerTime
			dateTimeString = TimeHelper.FormattedDate(passedTime);

			Updated?.Invoke(this);
		}

		public void OnCompleted()
		{
			Completed?.Invoke(this);
		}
	}
}