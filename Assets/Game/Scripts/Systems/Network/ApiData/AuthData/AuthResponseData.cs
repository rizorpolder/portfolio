using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.AuthData
{
	public class AuthResponseData : IParse<AuthResponseData>
	{
		public AuthUserData UserData;

		public AuthResponseData ParseFrom(byte[] responseData)
		{
			var str = Encoding.UTF8.GetString(responseData);
			var data = JsonConvert.DeserializeObject<AuthUserData>(str);
			UserData = data;
			return this;
		}
	}
}