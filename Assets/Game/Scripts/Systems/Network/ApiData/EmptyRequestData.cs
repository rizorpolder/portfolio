namespace Game.Scripts.Systems.Network.ApiData
{
	public class EmptyRequestData : IMessage
	{
		public byte[] ToByteArray()
		{
			return new byte[] { };
		}
	}
}