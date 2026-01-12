using System;

namespace Game.Scripts.Systems.Network
{
	[Serializable]
	public class AuthUserData
	{
		public int sid;
		public int localId;
		public string externalId;
		public string name;
		public string surname;
	}
}