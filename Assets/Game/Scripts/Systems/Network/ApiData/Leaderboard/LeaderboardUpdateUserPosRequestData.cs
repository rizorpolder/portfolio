using System.Text;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.Leaderboard
{
	public class LeaderboardUpdateUserPosRequestData : IMessage
	{
		public int score;

		public byte[] ToByteArray()
		{
			string json = JsonConvert.SerializeObject(this);
			return Encoding.UTF8.GetBytes(json);
		}
	}
}