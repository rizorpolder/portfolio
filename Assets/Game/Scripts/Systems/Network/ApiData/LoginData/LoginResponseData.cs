using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.LoginData
{
	public class LoginResponseData : IParse<LoginResponseData>
	{
		public int sid;

		public LoginResponseData ParseFrom(byte[] responseData)
		{
			var str = Encoding.UTF8.GetString(responseData);
			var data = JsonConvert.DeserializeObject<LoginResponseData>(str);
			sid = data.sid;
			return this;
		}
	}
}