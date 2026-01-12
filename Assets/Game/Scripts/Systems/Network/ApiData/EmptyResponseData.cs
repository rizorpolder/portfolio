namespace Game.Scripts.Systems.Network.ApiData
{
	public class EmptyResponseData : IParse<EmptyResponseData>
	{
		public EmptyResponseData ParseFrom(byte[] responseData)
		{
			return null;
		}
	}
}