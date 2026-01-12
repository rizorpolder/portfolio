using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.Leaderboard;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class LeaderboardRatingUpdateRequest : PostRequest<LeaderboardUpdateUserPosRequestData, EmptyResponseData>
	{
		protected override string RelativePath => "rating/update";

		public LeaderboardRatingUpdateRequest(LeaderboardUpdateUserPosRequestData requestData) :
			base(requestData)
		{
		}

		protected override void SetHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("Content-Type", "application/json");
		}

		protected override EmptyResponseData ParseResponseData(byte[] data)
		{
			return new EmptyResponseData().ParseFrom(data);
		}
	}
}