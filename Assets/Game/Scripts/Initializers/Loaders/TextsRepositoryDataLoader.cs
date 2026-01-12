using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Systems.Network;
using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.RestApi.Requests;
using Game.Scripts.Systems.Texts;
using Game.Scripts.Systems.Texts.Configs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Initialize.Loaders
{
	public class TextsRepositoryDataLoader : IDataLoader
	{
		private const float RequestTimeoutInSec = 6f;

		[Inject] private RequestsFactory _requestsFactory;
		[Inject] private TextsRepositoryConfig _textsRepositoryConfig;
		[Inject] private NetworkConfig _networkConfig;
		private readonly IDataCache<TextsRepository> _cache;

		public TextsRepositoryDataLoader(IDataCache<TextsRepository> cache)
		{
			_cache = cache;
		}

		public async UniTask LoadAsync()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable || !_networkConfig.SyncWithServer)
			{
				LoadLocalData();
				return;
			}


			await LoadServerData();
		}

		private async UniTask LoadServerData()
		{
			try
			{
				var req = _requestsFactory.CreateRequest<GetAllTextsRequest, EmptyRequestData>(new EmptyRequestData());
				var task = req.Make();

				var (hasResultLeft, response) =
					await UniTask.WhenAny(task, UniTask.Delay(TimeSpan.FromSeconds(RequestTimeoutInSec)));

				if (hasResultLeft)
					_cache.SetData(response.Data.GenerateConfig());
				else
				{
					LoadLocalData();
				}
			}
			catch (Exception e)
			{
				Debug.Log(e);
				LoadLocalData();
			}
		}

		private void LoadLocalData()
		{
			_cache.SetData(_textsRepositoryConfig.GenerateConfig()); //from SO
		}
	}
}