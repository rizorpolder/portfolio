using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.AllTextsData;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class GetAllTextsRequest : GetRequest<EmptyRequestData, TextsResponseData>
	{
		private string _relativePath => "texts/list-for-game";

		public GetAllTextsRequest(EmptyRequestData requestBodyBodyData) : base(requestBodyBodyData)
		{
		}

		protected override string RelativePath => _relativePath;

		protected override TextsResponseData ParseResponseData(byte[] data)
		{
			return new TextsResponseData().ParseFrom(data);
		}
	}
}