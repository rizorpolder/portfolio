using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.Leaderboard
{
	public class LeaderboardTopResponseData : IParse<LeaderboardTopResponseData>
	{
		public List<PlayerPositionData> leaderboard;
		
		public LeaderboardTopResponseData ParseFrom(byte[] responseData)
		{
			var str = Encoding.UTF8.GetString(responseData);
			return JsonConvert.DeserializeObject<LeaderboardTopResponseData>(str);
			
		}
	}
}