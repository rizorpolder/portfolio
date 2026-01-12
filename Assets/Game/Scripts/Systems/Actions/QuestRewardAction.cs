using System;
using Game.Scripts.Systems.Actions.Data;
using Game.Scripts.Systems.Actions.Interfaces;
using Game.Scripts.Systems.QuestSystem;
using Zenject;

namespace Game.Scripts.Systems.Actions
{
	public class QuestRewardAction : BaseAction<QuestRewardData>
	{
		public override event Action<IActionExecutable> OnActionExecuted;
		
		[Inject] private IQuestCommand _questCommand;
		public override void Execute()
		{
			_questCommand.QuestResourceAction(_data);
			OnActionExecuted?.Invoke(this);
		}
	}
}