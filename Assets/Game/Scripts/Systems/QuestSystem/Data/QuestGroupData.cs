using System.Collections.Generic;
using Game.Scripts.Data;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	public class QuestGroupData
	{
		public string ID;
		public int ActorID;
		public string MaxDescriptionID;
		public string LowDescriptionID;
		public Resource StartReward;
		public Resource CurrentReward;

		private Dictionary<string, bool> _changeRewardReason = new Dictionary<string, bool>();

		public QuestGroupData(QuestGroupData entryData)
		{
			ID = entryData.ID;
			CurrentReward = entryData.CurrentReward;
		}

		public QuestGroupData(QuestGroupConfig config)
		{
			ID = config.GroupID;
			ActorID = config.ActorID;
			StartReward = (Resource)config.Reward.Clone();
			CurrentReward = (Resource)config.Reward.Clone();
			MaxDescriptionID = config.MaxDescriptionID;
			LowDescriptionID = config.LowDescriptionID;
		}

		public void ReduceQuestGroupReward(string reason, Resource delta)
		{
			ChangeResourceValue(reason, delta, false);
		}

		public void AddQuestGroupReward(string reason, Resource delta)
		{
			ChangeResourceValue(reason, delta, true);
		}

		private void ChangeResourceValue(string reason, Resource delta, bool sign)
		{
			if (!_changeRewardReason.TryAdd(reason, true))
				return;

			if (!CurrentReward.Type.Equals(delta.Type))
				return;


			if (!sign)
			{
				CurrentReward.Value -= delta.Value;
			}
			else
			{
				CurrentReward.Value += delta.Value;
			}

			_changeRewardReason[reason] = sign;
		}
	}
}