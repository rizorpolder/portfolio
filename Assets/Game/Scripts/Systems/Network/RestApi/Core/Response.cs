namespace Game.Scripts.Systems.Network.RestApi
{
	public class Response<TResponseData>
	{
		public bool IsSuccess;
		public TResponseData Data;

		private Response()
		{
			IsSuccess = false;
		}

		private Response(TResponseData data)
		{
			IsSuccess = true;
			Data = data;
		}

		public static Response<TResponseData> Failed()
		{
			return new Response<TResponseData>();
		}

		public static Response<TResponseData> Success(TResponseData data)
		{
			return new Response<TResponseData>(data);
		}
	}
}