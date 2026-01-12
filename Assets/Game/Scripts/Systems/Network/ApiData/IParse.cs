namespace Game.Scripts.Systems.Network.ApiData
{
	public interface IParse<T>
	{
		public T ParseFrom(byte[] responseData);
	}
}