using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.ServerTimeData
{
	public class ServerTimeResponseData : IParse<ServerTimeResponseData>
	{
		public long timestamp;
		public string iso;

		public ServerTimeResponseData ParseFrom(byte[] responseData)
		{
			var jsn = Encoding.UTF8.GetString(responseData);
			var obj = JsonConvert.DeserializeObject<ServerTimeResponseData>(jsn);
			return obj;
		}
	}
}