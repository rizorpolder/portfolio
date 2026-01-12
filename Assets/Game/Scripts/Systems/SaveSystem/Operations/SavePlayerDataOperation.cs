
namespace Game.Scripts.Systems.SaveSystem.Operations
{
	public class SavePlayerDataOperation : ASaveOperation
	{
		

		public SavePlayerDataOperation(IDataSerializer serializer, IDataSaver saver) : base(serializer, saver)
		{
		}

		protected override string FileName => nameof(SaveDataType.PlayerData);

		protected override string GetData()
		{
			return null;
		}
	}
}