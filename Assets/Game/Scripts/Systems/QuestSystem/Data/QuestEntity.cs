using Game.Scripts.Data;
using Game.Scripts.Systems.DialogueSystem.Data;

namespace Game.Scripts.Systems.QuestSystem.Data
{
	public class QuestEntity
	{
		public readonly string Name;
		
		private CustomAction _successAction;
		public CustomAction SuccessAction => _successAction;
		public bool HasSuccessAction => _successAction != null;
		
		private CustomAction _failureAction;
		public CustomAction FailureAction => _failureAction;
		public bool HasFailureAction => _failureAction != null;

		private string _groupName;
		public string GroupName => _groupName;
		
		private bool _isEndOfModule = false;
		public bool IsEndOfModule => _isEndOfModule;

		private bool _isLastGroupQuest = false;
		public bool IsLastGroupQuest => _isLastGroupQuest;
		
		public QuestEntity(string name)
		{
			Name = name;
		}
		
		public void ReplaceSuccessAction(CustomAction action)
		{
			_successAction = action;
		}

		public void ReplaceFailureAction(CustomAction action)
		{
			_failureAction = action;
		}
		
		public void SetEndOfModule()
		{
			_isEndOfModule = true;
		}
		
		public void SetQuestGroupName(string groupName)
		{
			_groupName = groupName;
		}
		
		public void SetLastGroupQuest()
		{
			_isLastGroupQuest = true;
		}
	}
}