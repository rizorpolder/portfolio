using Game.Scripts.Systems.SaveSystem.Operations;
using Zenject;

namespace Game.Scripts.Systems.SaveSystem
{
	public class SaveOperationFactory
	{
		private DiContainer _container;

		private IDataSaver _saver;
		private IDataSerializer _serializer;

		[Inject]
		public SaveOperationFactory(DiContainer diContainer, IDataSaver saver, IDataSerializer serializer)
		{
			_container = diContainer;
			_saver = saver;
			_serializer = serializer;
		}

		public ASaveOperation CreateSaveOperation(SaveDataType type)
		{
			return type switch
			{
				SaveDataType.PlayerData => _container.Instantiate<SavePlayerDataOperation>(new object[] {_serializer, _saver}),
				SaveDataType.QuestData => _container.Instantiate<SaveQuestDataOperation>(new object[] {_serializer, _saver}),
				SaveDataType.Statistics => _container.Instantiate<SaveStatisticOperation>(new object[] {_serializer, _saver}),
				_ => null
			};
		}
	}
}