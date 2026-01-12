using System;
using System.Runtime.InteropServices;
using System.Threading;
using AudioManager.Runtime.Extensions;
using Cysharp.Threading.Tasks;
using Game.Scripts.Helpers;
using Game.Scripts.Systems.Network.ApiData;
using Game.Scripts.Systems.Network.ApiData.Leaderboard;
using Game.Scripts.Systems.Network.ApiData.LoginData;
using Game.Scripts.Systems.Network.ApiData.PlayerData;
using Game.Scripts.Systems.Network.Data;
using Game.Scripts.Systems.Network.RestApi.Requests;
using Game.Scripts.Systems.PlayerController;
using Game.Scripts.Systems.SaveSystem;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Game.Scripts.Systems.Network
{
	public class NetworkSystem : IInitializable, IDisposable
	{
		private const string CredentialToken = "credential";

		public AuthUserData AuthUserData;
		public bool IsAuthorized => AuthUserData != null;

		[Inject] private IPlayerListener _playerListener;
		[Inject] private IPlayerData _playerData;

		[Inject] private RequestsFactory _requestsFactory;
		[Inject] private NetworkConfig _networkConfig;
		[Inject] private ISaveSystemCommand _saveSystemCommand;

		private ServerTime _serverTime;

		public ServerTime ServerTime => _serverTime;

		public void Initialize()
		{
			_playerListener.OnPlayerPointChanged += UpdateScore;
		}

		public async UniTask StartLoadUser()
		{
			if (!_networkConfig.SyncWithServer)
			{
				return;
			}

			await InitServerTime();
			await LoadUser();
		}

		private async UniTask LoadUser(CancellationTokenSource token = null)
		{
			try
			{
				AuthUserData = await AuthPlayer();


				if (AuthUserData == null)
				{
					ResetToken();
					return;
				}

				var credentials = PlayerPrefs.GetString(CredentialToken);
				if (credentials.IsNullOrEmpty())
				{
					AuthUserData = null;
					ResetToken();
					return;
				}

				_requestsFactory.UpdateCookie(credentials);
				await OnSuccessAuth();
			}
			catch (Exception e)
			{
				Debug.Log($"[NETWORK] Not Authorized {e.Message}");
			}
		}

		private async UniTask OnSuccessAuth()
		{
			if (Application.isEditor && !_networkConfig.SyncWithServer)
				return;

			await _saveSystemCommand.SyncData();
		}

		private async UniTask<AuthUserData> AuthPlayer()
		{
			var req = _requestsFactory.CreateRequest<AuthRequest, EmptyRequestData>(new EmptyRequestData());
			var resp = await req.Make();

			return !resp.IsSuccess ? null : resp.Data.UserData;
		}

		public async UniTask Login(string userName, string userSurname)
		{
			if (!_networkConfig.SyncWithServer)
				return;

			var reqData = new LoginRequestData
			{
				name = userName,
				surname = userSurname
			};

			var _request = new UnityWebRequest($"{_networkConfig.GetRestApiUri()}auth/login", "POST");

			var uploadHandler = new UploadHandlerRaw(reqData.ToByteArray());
			_request.uploadHandler = uploadHandler;
			_request.downloadHandler = new DownloadHandlerBuffer();
			_request.SetRequestHeader("Content-Type", "application/json");

			await _request.SendWebRequest();
			var setCookie = _request.GetResponseHeader("Set-Cookie");

#if UNITY_WEBGL
			if (setCookie.IsNullOrEmpty())
				setCookie = GetCookies();
#endif


			if (!string.IsNullOrEmpty(setCookie))
			{
				PlayerPrefs.SetString(CredentialToken, setCookie);
				PlayerPrefs.Save();
				_requestsFactory.UpdateCookie(setCookie);

				LoadUser().Forget();
				return;
			}

			Debug.LogAssertionFormat("[NETWORK] Cookies failed");
		}

		private void UpdateScore(int prev, int score)
		{
			if (!_networkConfig.SyncWithServer)
				return;

			SendPlayerScore(score).Forget();
		}

		private async UniTask SendPlayerScore(int score)
		{
			var data = new LeaderboardUpdateUserPosRequestData
			{
				score = score
			};

			var req =
				_requestsFactory
					.CreateRequest<LeaderboardRatingUpdateRequest, LeaderboardUpdateUserPosRequestData>(data);
			var resp = await req.Make();
			if (resp.IsSuccess)
			{
				Debug.Log("Score updated: " + score);
			}
		}

		public async UniTask<string> LoadPlayerData()
		{
			var req = _requestsFactory.CreateRequest<LoadPlayerDataRequest, EmptyRequestData>(new EmptyRequestData());
			var resp = await req.Make();
			if (resp.IsSuccess)
			{
				return resp.Data.data;
			}

			Debug.LogAssertionFormat($"[NETWORK] Loading player data failed");

			return null;
		}

		public async UniTask SavePlayerData(SavePlayerRequestData playerData)
		{
			var req =
				_requestsFactory.CreateRequest<SavePlayerDataRequest, SavePlayerRequestData>(
					playerData);
			var resp = await req.Make();
			if (resp.IsSuccess)
			{
				Debug.Log("[NETWORK] On Player Data Saved");
			}
			else
				Debug.Log("[NETWORK] On Player Data NOT Saved");
		}

		private void ResetToken()
		{
			PlayerPrefs.DeleteKey(CredentialToken);
		}

		public void Disconnect()
		{
			AuthUserData = null;
			_requestsFactory.UpdateCookie("");
			ResetToken();
			UnityWebRequest.ClearCookieCache(new Uri(_networkConfig.GetRestApiUri()));
		}

		private async UniTask InitServerTime()
		{
			var request = _requestsFactory.CreateRequest<ServerTimeRequest, EmptyRequestData>(new EmptyRequestData());
			try
			{
				var serverTimeResponse = await request.Make();
				var serverTime = TimeHelper.FromUnixTimeMs(serverTimeResponse.Data.timestamp);
				_serverTime = new ServerTime(serverTime);
			}
			catch (Exception ex)
			{
				Debug.LogError($"could not get server time!\n{ex.Message}");
				_serverTime = new ServerTime();
			}
		}

		public void Dispose()
		{
			_playerListener.OnPlayerPointChanged -= UpdateScore;
		}

		[DllImport("__Internal")]
		private static extern string GetCookies();
	}
}