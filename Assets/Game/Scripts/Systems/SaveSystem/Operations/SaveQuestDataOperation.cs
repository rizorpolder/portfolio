using Game.Scripts.Systems.QuestSystem;
using Zenject;

namespace Game.Scripts.Systems.SaveSystem.Operations
{
	public class SaveQuestDataOperation : ASaveOperation
	{
		[Inject] private IQuestCommand _questCommand;

		public SaveQuestDataOperation(IDataSerializer serializer, IDataSaver saver) : base(serializer, saver)
		{
		}

		protected override string FileName => nameof(SaveDataType.QuestData);

		protected override string GetData()
		{
			var data = _questCommand.GetQuestSaveData();
			return _serializer.Serialize(data);
		}
	}
}