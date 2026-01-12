using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.PlayerData
{
	public class SavePlayerRequestData : IMessage
	{
		public string playerData;

		public byte[] ToByteArray()
		{
			var json = JsonConvert.SerializeObject(this);
			return Encoding.UTF8.GetBytes(json);
		}
	}
}