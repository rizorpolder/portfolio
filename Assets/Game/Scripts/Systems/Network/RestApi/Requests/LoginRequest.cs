using Game.Scripts.Systems.Network.ApiData.LoginData;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class LoginRequest : PostRequest<LoginRequestData, LoginResponseData>
	{
		protected override string RelativePath => "auth/login";

		public LoginRequest(LoginRequestData requestData) : base(requestData)
		{
		}

		protected override void SetHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("Content-Type", "application/json");
		}

		protected override LoginResponseData ParseResponseData(byte[] data)
		{
			return new LoginResponseData().ParseFrom(data);
		}
	}
}