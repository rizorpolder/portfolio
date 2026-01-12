using System.Collections.Generic;
using Game.Scripts.Systems.Texts.Configs;
using UnityEngine;

namespace Game.Scripts.Systems.Texts
{
	public class TextsRepository
	{
		private Dictionary<TextGroupType, Dictionary<string, string>> _repository =
			new Dictionary<TextGroupType, Dictionary<string, string>>();

		public bool TryAddData(TextGroupType groupKey, string entryKey, string entryValue)
		{
			if (!_repository.ContainsKey(groupKey))
			{
				_repository.Add(groupKey, new Dictionary<string, string>());
			}

			if (_repository[groupKey].ContainsKey(entryKey))
			{
				Debug.Log($"Key {entryKey} already exists in group {groupKey}");
				return false;
			}

			_repository[groupKey].Add(entryKey, entryValue);
			return true;
		}

		public string GetText(TextGroupType groupKey, string entryKey)
		{
			if (_repository.TryGetValue(groupKey, out var group))
			{
				if (group.TryGetValue(entryKey, out var value))
				{
					return value;
				}
			}

			Debug.Log($"Text not found for group {groupKey}/{entryKey}");
			return string.Empty;
		}
	}
}