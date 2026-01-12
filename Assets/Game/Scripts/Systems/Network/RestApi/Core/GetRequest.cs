using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Network.ApiData;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi
{
	public abstract class GetRequest<TRequest, TResponseData> : Request<TResponseData>, IDisposable
		where TRequest : IMessage where TResponseData : IParse<TResponseData>
	{
		protected override string MethodType => "GET";

		private readonly TRequest requestBodyBodyData;
		private UnityWebRequest _request;

		protected GetRequest(TRequest requestBodyBodyData)
		{
			this.requestBodyBodyData = requestBodyBodyData;
		}

		public override async UniTask<Response<TResponseData>> Make()
		{
			return await MakeRequest(FullUri);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="url">Target prefix path </param>
		/// <returns></returns>
		public override async UniTask<Response<TResponseData>> Make(string url)
		{
			return await MakeRequest(url + RelativePath);
		}

		public override async UniTask<Response<TResponseData>> Make(CancellationTokenSource cancellationToken = null)
		{
			return await MakeRequest(FullUri, cancellationToken);
		}

		protected override async UniTask<Response<TResponseData>> MakeRequest(string url,
			CancellationTokenSource tokenSource = null)
		{
			_request = new UnityWebRequest(url, MethodType);

			var uploadHandler = new UploadHandlerRaw(requestBodyBodyData.ToByteArray());
			_request.uploadHandler = uploadHandler;
			_request.downloadHandler = new DownloadHandlerBuffer();
			SetHeaders(_request);
			SetCertificate(_request);
			try
			{
				await _request.SendWebRequest().WithCancellation(tokenSource?.Token ?? CancellationToken.None);

			}
			catch (OperationCanceledException)
			{
				if (!_request.isDone)
					_request.Abort();

				return Response<TResponseData>.Failed();
			}

			return ParseResponse(_request);
		}

		protected virtual void SetHeaders(UnityWebRequest request)
		{
			request.SetRequestHeader("Cache-Control", "no-cache");
		}

		protected virtual void SetCertificate(UnityWebRequest request)
		{
		}

		private Response<TResponseData> ParseResponse(UnityWebRequest request)
		{
			if (request.result != UnityWebRequest.Result.Success)
			{
				return Response<TResponseData>.Failed();
			}

			var data = default(TResponseData);
			try
			{
				data = ParseResponseData(request.downloadHandler.data ?? Array.Empty<byte>());
			}
			catch (Exception e)
			{
				_ = e;
				// ignore
			}

			return Response<TResponseData>.Success(data);
		}

		protected abstract TResponseData ParseResponseData(byte[] data);

		public void Dispose()
		{
			_request?.Dispose();
		}
	}
}