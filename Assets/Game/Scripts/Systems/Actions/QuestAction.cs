using System;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.QuestSystem;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	[Serializable]
	public class QuestAction : BaseAction<QuestActionData>
	{
		public override event Action<IActionExecutable> OnActionExecuted;

		[Inject] private IQuestCommand _command;

		public override void Execute()
		{
			_command.ChangeQuestState(_data.QuestID, _data.QuestState);
			OnActionExecuted?.Invoke(this);
		}
	}
}