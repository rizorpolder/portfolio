using Newtonsoft.Json;

namespace Game.Scripts.Systems.SaveSystem.Serializer
{
	public class JsonDataSerializer : IDataSerializer
	{
		public T Deserialize<T>(string data)
		{
			return JsonConvert.DeserializeObject<T>(data);
		}

		public string Serialize<T>(T data)
		{
			var result = JsonConvert.SerializeObject(data);
			return result;
		}
	}
}