using System;

namespace Game.Scripts.Systems.Network
{
	[Serializable]
	public struct NetworkPath
	{
		public NetworkServer server;
		public string adminUri;
		public NetworkProtocol protocol;
		public string uri;
	}
	
	public enum NetworkServer
	{
		Development,
		LocalHost,
		LocalNetwork,
		Production,
	}
}