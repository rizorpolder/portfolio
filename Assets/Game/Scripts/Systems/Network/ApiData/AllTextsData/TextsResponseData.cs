using System;
using System.Collections.Generic;
using System.Text;
using Game.Scripts.Systems.Texts;
using Game.Scripts.Systems.Texts.Configs;
using Newtonsoft.Json;

namespace Game.Scripts.Systems.Network.ApiData.AllTextsData
{
	public class TextsResponseData : IParse<TextsResponseData>
	{
		public List<TextsResponseDataWrapper> data;

		public TextsResponseData ParseFrom(byte[] responseData)
		{
			var result = Encoding.UTF8.GetString(responseData);
			var obj = JsonConvert.DeserializeObject<TextsResponseData>(result);

			this.data = obj.data;
			return this;
		}

		public TextsRepository GenerateConfig()
		{
			var rep = new TextsRepository();
			foreach (var wrapper in data)
			{
				foreach (var textEntry in wrapper.data)
				{
					if (!Enum.TryParse(wrapper.type, out TextGroupType textGroup))
						continue;

					rep.TryAddData(textGroup, textEntry.token, textEntry.text);
				}
			}

			return rep;
		}
	}

	public class TextsResponseDataWrapper
	{
		public string type;
		public List<TextsResponseDataEntry> data;
	}

	public class TextsResponseDataEntry
	{
		public string token;
		public string text;
	}
}