using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Systems.Google;
using UnityEngine;

namespace Game.Scripts.Systems.Texts.Configs
{
	[CreateAssetMenu(fileName = "TextsRepositoryConfig", menuName = "Project/TextsRepositoryConfig")]
	public class TextsRepositoryConfig : AGoogleSheetRemoteConfig
	{
		[SerializeField] public List<TextGroup> _textGroups = new List<TextGroup>();

		public void TryAddEntry(TextGroupType groupKey, string entryKey, string entryValue)
		{
			var group = _textGroups.FirstOrDefault(x => x.GroupType.Equals(groupKey));
			if (group == null)
			{
				group = new TextGroup() {GroupType = groupKey};
				_textGroups.Add(group);
			}

			var entry = group.TextEntries.FirstOrDefault(x => x.Key.Equals(entryKey));
			if (entry != null)
			{
				entry.Value = entryValue;
				return;
			}

			entry = new TextEntry() {Key = entryKey, Value = entryValue};
			group.TextEntries.Add(entry);
		}

		public TextsRepository GenerateConfig()
		{
			var rep = new TextsRepository();
			foreach (var group in _textGroups)
			{
				foreach (var textEntry in group.TextEntries)
				{
					rep.TryAddData(group.GroupType, textEntry.Key, textEntry.Value);
				}
			}

			return rep;
		}

		public override void Clear()
		{
			_textGroups.Clear();
		}


	}

	[Serializable]
	public class TextGroup
	{
		public TextGroupType GroupType;
		public List<TextEntry> TextEntries = new List<TextEntry>();
	}

	[Serializable]
	public class TextEntry
	{
		public string Key;
		public string Value;
	}

	public enum TextGroupType
	{
		Common,
		Quest,
		QuestEntry,
		QuestModule,
		QuestName,
		QuestGoal,
		QuestDialogue,
		Choice,
		PopupText,
		MentorDialogue,
		Tutorial,
		TestModule
	}
}