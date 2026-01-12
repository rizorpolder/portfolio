using System;
using Game.Scripts.Systems.QuestSystem.Data;

namespace Game.Scripts.Systems.QuestSystem
{
	public interface IQuestListener
	{
		public event Action<string> OnQuestStateChanged;
		public event Action<string> OnQuestEntryStateChanged;
	}
}