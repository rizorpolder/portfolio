using System;
using Game.Scripts.Data;

namespace Game.Scripts.Systems.QuestSystem
{
	[Serializable]
	public class QuestGroupConfig
	{
		public string GroupID;
		public int ActorID;
		public string MaxDescriptionID;
		public string LowDescriptionID;
		public Resource Reward;
	}
}