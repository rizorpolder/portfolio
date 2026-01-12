using System.Collections.Generic;
using Game.Scripts.Systems.Character.Reaction.Data;
using UnityEngine;

namespace Game.Scripts.Systems.Character.Reaction
{
	public class MonologueReaction : BaseReaction
	{
		private List<int> probabilties = new List<int>();

		public override void Start(string target)
		{
			_target = target;
			var data = reactionData as MonologueReactionData;

			CheckSize(data);

			float randomValue = Random.Range(0, 100);

			string token = "";
			int probability = 0;
			for (int i = 0; i < data.tokens.Count; ++i)
			{
				probability += probabilties[i];

				if (randomValue < probability)
				{
					token = data.tokens[i];
					FixProbabilty(i);
					break;
				}
			}

			string pathCommand = $"Bark({target}, {token});";
			//
			// sequencer = DialogueManager.PlaySequence(pathCommand);
			// sequencer.FinishedSequenceHandler += OnFinish;
		}

		/// <summary>
		///  Уменьшает вероятность выбранной реплики
		/// </summary>
		/// <param name="index">Номер реплики</param>
		private void FixProbabilty(int index)
		{
			int x2Delta = probabilties[index] / 2;
			int divider = probabilties.Count > 1 ? probabilties.Count - 1 : 1;
			int upDelta = (probabilties[index] - x2Delta) / divider;
			int fault = 0;

			for (int i = 0; i < probabilties.Count; ++i)
			{
				if (i == index)
				{
					probabilties[i] -= x2Delta;
					if (this.probabilties[i] < 0)
						this.probabilties[i] = 0;
				}
				else
				{
					probabilties[i] += upDelta;
					if (this.probabilties[i] > 100)
						this.probabilties[i] = 100;
				}

				fault += this.probabilties[i];
			}

			this.probabilties[0] += (100 - fault);
		}

		/// <summary>
		/// Проверка размера массива вероятностей и его заполнение
		/// </summary>
		/// <param name="data">Данные для проверки, токены локализации</param>
		private void CheckSize(MonologueReactionData data)
		{
			if (probabilties.Count < data.tokens.Count)
			{
				int fault = 0;
				probabilties.Clear();
				for (int i = 0; i < data.tokens.Count; ++i)
				{
					var value = 100 / data.tokens.Count;
					fault += value;
					probabilties.Add(value);
				}

				this.probabilties[this.probabilties.Count - 1] += (100 - fault);
			}
		}
	}
}