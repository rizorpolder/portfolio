using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.ServerTimeData;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class ServerTimeRequest : GetRequest<EmptyRequestData, ServerTimeResponseData>
	{
		private string _relativePath;

		public ServerTimeRequest(EmptyRequestData requestData) : base(requestData)
		{
		}

		protected override string RelativePath => "app/clock";

		protected override ServerTimeResponseData ParseResponseData(byte[] data)
		{
			return new ServerTimeResponseData().ParseFrom(data);
		}
	}
}