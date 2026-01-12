using System.Text;
using Game.Scripts.Systems.Network.ApiData;
using Newtonsoft.Json;

namespace DefaultNamespace
{
	public class LoadPlayerResponseData : IParse<LoadPlayerResponseData>
	{
		public string id;
		public string data;

		public LoadPlayerResponseData ParseFrom(byte[] responseData)
		{
			var result = Encoding.UTF8.GetString(responseData);
			var obj = JsonConvert.DeserializeObject<LoadPlayerResponseData>(result);
			id = obj.id;
			data = obj.data;
			return this;
		}
	}
}