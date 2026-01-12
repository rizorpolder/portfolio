using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.LoginData
{
	public class LoginRequestData : IMessage
	{
		public string name;
		public string surname;

		public byte[] ToByteArray()
		{
			string json = JsonConvert.SerializeObject(this);
			return Encoding.UTF8.GetBytes(json);
		}
	}
}