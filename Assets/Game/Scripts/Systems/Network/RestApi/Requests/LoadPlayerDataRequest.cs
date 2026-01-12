using DefaultNamespace;
using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.PlayerData;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class LoadPlayerDataRequest : PostRequest<EmptyRequestData, LoadPlayerResponseData>
	{
		protected override string RelativePath => "player-data/load";

		public LoadPlayerDataRequest(EmptyRequestData requestData) : base(requestData)
		{
		}
		
		protected override void SetHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("Content-Type", "application/json");
		}
		
		protected override LoadPlayerResponseData ParseResponseData(byte[] data)
		{
			return new LoadPlayerResponseData().ParseFrom(data);
		}
	}
}