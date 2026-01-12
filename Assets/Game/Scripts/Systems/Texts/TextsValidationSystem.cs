using System;
using Game.Scripts.Systems.Initialize.Signals;
using Game.Scripts.Systems.Texts.Configs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Systems.Texts
{
	public class TextsValidationSystem : IInitializable
	{
		private TextsRepository _textsRepository;

		[Inject] private  TextsRepositoryConfig _textsRepositoryConfig;
		[Inject] private SignalBus _signalBus;
		[Inject] private IDataCache<TextsRepository> _textsDataCache;
		
		public void Initialize()
		{
			_signalBus.Subscribe<CacheUpdatedSignal>(OnCacheReadyHandler);
			_textsRepository = _textsRepositoryConfig.GenerateConfig();
		}
		private void OnCacheReadyHandler()
		{
			_textsRepository = _textsDataCache.GetData();
		}
		
		public string GetLocalizedString(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				return "";
			}

			var result = token.Split('/', StringSplitOptions.RemoveEmptyEntries);
			TextGroupType group;
			string key;
			if (result.Length == 0)
			{
#if UNITY_EDITOR
				Debug.Log($"Token {token} is empty");
#endif
				return "Ooops....";
			}

			if (result.Length > 1)
			{
				if (!Enum.TryParse(result[0], true, out group))
				{
#if UNITY_EDITOR
					Debug.Log($"Group {result[0]} doesn't exist");
#endif
					return "Ooops....";
				}

				key = result[1];
			}
			else
			{
				group = TextGroupType.Common;
				key = result[0];
			}

			return _textsRepository.GetText(group, key);
		}
		
		public string GetLocalizedString(string token, string param, string value)
		{
			var resultToken = GetLocalizedString(token);
			return resultToken.Replace($"%{param}%", value);
		}
	}
}