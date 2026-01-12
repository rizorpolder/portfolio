using System;
using Game.Scripts.Data;
using Game.Scripts.Systems.Actions.Interfaces;

namespace Game.Scripts.Systems.Actions.Data
{
	[Serializable]
	public class QuestRewardData : IActionData
	{
		public string moduleID;
		public Resource Resource;
		public QuestRewardActionType Type = QuestRewardActionType.Decrease;
		public string Token;
	}

	public enum QuestRewardActionType
	{
		Increase,
		Decrease,
	}
}