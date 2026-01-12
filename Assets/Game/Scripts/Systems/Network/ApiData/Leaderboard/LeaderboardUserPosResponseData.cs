using System;
using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.Leaderboard
{
	public class LeaderboardUserPosResponseData : IParse<LeaderboardUserPosResponseData>
	{
		public PlayerPositionData data;

		public LeaderboardUserPosResponseData ParseFrom(byte[] responseData)
		{
			var str = Encoding.UTF8.GetString(responseData);
			var obj = JsonConvert.DeserializeObject<PlayerPositionData>(str);
			data = obj;
			return this;
		}
	}

	[Serializable]
	public class PlayerPositionData
	{
		public string accountId;
		public int position;
		public int score;
	}
}