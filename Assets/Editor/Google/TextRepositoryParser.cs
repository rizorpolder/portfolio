using System;
using Game.Scripts.Systems.Texts.Configs;

namespace Editor.Google
{
	public class TextRepositoryParser : GoogleSheetParser<TextsRepositoryConfig>
	{
		public TextRepositoryParser(TextsRepositoryConfig config) : base(config)
		{
		}

		protected override void ImportEntry(string[] data)
		{
			if (Enum.TryParse(data[0], out TextGroupType group))
			{
				_config.TryAddEntry(group, data[1], data[2]);
			}
			else
			{
				if (string.IsNullOrEmpty(data[0]))
				{
					_config.TryAddEntry(TextGroupType.Common, data[1], data[2]);
				}
			}
		}
	}
}