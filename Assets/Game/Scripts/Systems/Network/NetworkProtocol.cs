namespace Game.Scripts.Systems.Network
{
	public enum NetworkProtocol
	{
		Http,
		Https,
	}

	public static class NetworkProtocolExtensions
	{
		public static string ToUriString(this NetworkProtocol protocol)
		{
			switch (protocol)
			{
				case NetworkProtocol.Https:
					return "https://";
				case NetworkProtocol.Http:
				default:
					return "http://";
			}
		}
	}
}