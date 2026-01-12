using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.PlayerData;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class SavePlayerDataRequest : PostRequest<SavePlayerRequestData, EmptyResponseData>
	{
		public SavePlayerDataRequest(SavePlayerRequestData requestData) : base(requestData)
		{
		}

		protected override void SetHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("Content-Type", "application/json");
		}

		protected override string RelativePath => "player-data/save";

		protected override EmptyResponseData ParseResponseData(byte[] data)
		{
			return new EmptyResponseData().ParseFrom(data);
		}
	}
}