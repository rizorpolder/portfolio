using System;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	public enum QuestState
	{
		Unassigned = 0,
		Active = 1,
		Success = 2,
		Failed = 3,
		Grantable = 4, 
	}
}