using System;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.QuestSystem;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	[Serializable]
	public class QuestEntryAction : BaseAction<QuestEntryData>
	{
		public override event Action<IActionExecutable> OnActionExecuted;
		
		[Inject] private IQuestCommand _command;


		public override void Execute()
		{
			_command.SetEntryState(_data.QuestID, _data.EntryIndex, _data.NewEntryState);
			OnActionExecuted?.Invoke(this);

		}
	}
}