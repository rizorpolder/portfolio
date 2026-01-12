using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.Leaderboard;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class LeaderboardTopRequest : GetRequest<EmptyRequestData, LeaderboardTopResponseData>
	{
		protected override string RelativePath => $"rating/top10";

		public LeaderboardTopRequest(EmptyRequestData requestBodyBodyData) : base(requestBodyBodyData)
		{
		}

		protected override LeaderboardTopResponseData ParseResponseData(byte[] data)
		{
			return new LeaderboardTopResponseData().ParseFrom(data);
		}
	}
}