using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Game.Scripts.Systems.Network.RestApi
{
	public abstract class Request<TResponseData>
	{
		[Inject] private NetworkConfig _networkConfig;
		[Inject] protected RequestsFactory _requestsFactory;

		protected string Uri => $"{_networkConfig.GetRestApiUri()}";

		protected abstract string RelativePath { get; }

		protected string FullUri => $"{Uri}{RelativePath}";

		protected abstract string MethodType { get; }
		
		public abstract UniTask<Response<TResponseData>> Make();
		public abstract UniTask<Response<TResponseData>> Make(CancellationTokenSource cancellationToken = null);
		public abstract UniTask<Response<TResponseData>> Make(string url);
		protected abstract UniTask<Response<TResponseData>> MakeRequest(string url,CancellationTokenSource cancellationToken = null);
	}
}