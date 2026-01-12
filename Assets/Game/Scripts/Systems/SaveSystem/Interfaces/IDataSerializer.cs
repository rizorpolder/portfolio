namespace Game.Scripts.Systems.SaveSystem
{
	public interface IDataSerializer
	{
		//получает данные, возврщаает строку
		public T Deserialize<T>(string data);
		public string Serialize<T>(T data);
	}
}