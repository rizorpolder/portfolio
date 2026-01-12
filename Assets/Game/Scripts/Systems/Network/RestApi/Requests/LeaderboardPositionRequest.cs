using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.Leaderboard;
using Game.Scripts.Systems.PlayerController;
using Zenject;

namespace Game.Scripts.Systems.Network.RestApi.Requests
{
	public class LeaderboardPositionRequest : GetRequest<EmptyRequestData, LeaderboardUserPosResponseData>
	{
		[Inject] private IPlayerData _playerData;
		
		protected override string RelativePath => $"rating/position";

		public LeaderboardPositionRequest(EmptyRequestData requestBodyBodyData) : base(requestBodyBodyData)
		{
		}

		protected override LeaderboardUserPosResponseData ParseResponseData(byte[] data)
		{
			return new LeaderboardUserPosResponseData().ParseFrom(data);
		}
	}
}