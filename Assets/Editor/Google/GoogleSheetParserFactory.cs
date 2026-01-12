using System;
using Game.Scripts.Systems.Google;
using Game.Scripts.Systems.Texts.Configs;

namespace Editor.Google
{
	public static class GoogleSheetParserFactory
	{
		public static IGoogleSheetParser GetParser<T>(T config) where T : AGoogleSheetRemoteConfig
		{
			return config switch
			{
				TextsRepositoryConfig userConfig => new TextRepositoryParser(userConfig),
				_ => throw new NotSupportedException($"Тип {typeof(T).Name} не поддерживается")
			};
		}
	}
}