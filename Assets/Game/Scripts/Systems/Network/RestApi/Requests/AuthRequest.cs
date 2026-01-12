using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.AuthData;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class AuthRequest : GetRequest<EmptyRequestData, AuthResponseData>
	{
		protected override string RelativePath => "auth/me";

		public AuthRequest(EmptyRequestData requestBodyBodyData) : base(requestBodyBodyData)
		{
		}

		protected override AuthResponseData ParseResponseData(byte[] data)
		{
			return new AuthResponseData().ParseFrom(data);
		}
	}
}