using System;
using System.Threading;
using AudioManager.Runtime.Extensions;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Network.ApiData;
using UnityEngine.Networking;

namespace Game.Scripts.Systems.Network.RestApi
{
	public abstract class PostRequest<TRequest, TResponseData> : Request<TResponseData>, IDisposable
		where TRequest : IMessage
	{
		protected override string MethodType => "POST";

		private readonly TRequest _requestData;
		private UnityWebRequest _request;

		protected PostRequest(TRequest requestData)
		{
			_requestData = requestData;
		}

		public override async UniTask<Response<TResponseData>> Make()
		{
			return await MakeRequest(FullUri);
		}

		public override async UniTask<Response<TResponseData>> Make(CancellationTokenSource tokenSource = null)
		{
			return await MakeRequest(FullUri, tokenSource);
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

		protected override async UniTask<Response<TResponseData>> MakeRequest(string url,
			CancellationTokenSource tokenSource = null)
		{
			_request = new UnityWebRequest(url, MethodType);

			var uploadHandler = new UploadHandlerRaw(_requestData.ToByteArray());
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