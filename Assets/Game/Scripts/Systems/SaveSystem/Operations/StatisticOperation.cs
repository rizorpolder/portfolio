namespace Game.Scripts.Systems.SaveSystem.Operations
{
	public class SaveStatisticOperation : ASaveOperation
	{
		public SaveStatisticOperation(IDataSerializer serializer, IDataSaver saver) : base(serializer, saver)
		{
		}

		protected override string FileName => nameof(SaveDataType.Statistics);

		protected override string GetData()
		{
			return null;
		}
	}
}