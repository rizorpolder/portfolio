using System;
using System.Linq;
using Game.Scripts.Systems.DialogueSystem;
using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.DialogueSystemEditor
{
	[CustomEditor(typeof(ConversationConfig))]
	public class ConversationConfigEditor : UnityEditor.Editor
	{
		private ConversationConfig _target;

		private void OnEnable()
		{
			_target = target as ConversationConfig;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Conversation ID", EditorStyles.boldLabel);
			_target.ConversationID = EditorGUILayout.TextField(_target.ConversationID);

			EditorGUILayout.Space();

			var entriesList = serializedObject.FindProperty("DialogueEntries");
			entriesList.isExpanded = EditorGUILayout.Foldout(entriesList.isExpanded, "Dialogue Entries");

			if (entriesList.isExpanded)
			{
				for (int i = 0; i < _target.DialogueEntries.Count; i++)
				{
					DrawDialogueEntry(_target.DialogueEntries[i], i);
				}

				if (GUILayout.Button("Добавить"))
				{
					CreateDialogueEntryInstance();
				}
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

		private DialogueEntryConfig CreateDialogueEntryInstance(bool needCreateLink = true)
		{
			var newEntryInstance = new DialogueEntryConfig
			{
				ID = _target.DialogueEntries.Count,
			};

			newEntryInstance.Name = $"Entry_{newEntryInstance.ID}";
			_target.DialogueEntries.Add(newEntryInstance);

			if (_target.DialogueEntries.Count <= 1 || !needCreateLink)
				return newEntryInstance;

			var previousEntry = _target.DialogueEntries[newEntryInstance.ID - 1];
			var link = new DialogueLink
			{
				TargetEntryID = newEntryInstance.ID
			};
			previousEntry.NextEntriesLinks.Add(link);
			return newEntryInstance;
		}

		private void DrawDialogueEntry(DialogueEntryConfig entry, int index) //TODO Remove index, remove by EntryID
		{
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Entry ID: {_target.DialogueEntries[index].ID}", EditorStyles.miniBoldLabel);

			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				CreateEntryUnder(index);
				return;
			}

			if (GUILayout.Button("▲", GUILayout.Width(20)))
			{
				MoveEntry(index, index - 1);
				return;
			}

			if (GUILayout.Button("▼", GUILayout.Width(20)))
			{
				MoveEntry(index, index + 1);
				return;
			}

			if (GUILayout.Button("X", GUILayout.Width(20)))
			{
				RemoveEntry(index);
				return;
			}

			EditorGUILayout.EndHorizontal();

			entry.Name = EditorGUILayout.TextField("Entry name", entry.Name);
			entry.SpeakerID = EditorGUILayout.IntField("Speaker ID", entry.SpeakerID);
			entry.ListenerID = EditorGUILayout.IntField("Listener ID", entry.ListenerID);
			entry.DialogueTextToken = EditorGUILayout.TextField("Text Token", entry.DialogueTextToken);
			entry.IsMentorText = EditorGUILayout.Toggle("Is Mentor Panel", entry.IsMentorText);
			entry.AutoTransition = EditorGUILayout.Toggle("Auto Transit", entry.AutoTransition);
			// entry.EntryAction =
			// 	(ActionConfig) EditorGUILayout.ObjectField("Action", entry.EntryAction, typeof(ActionConfig), false);

			DrawEntryLinks(entry, index);
			DrawEntryEvents(entry, index);
			EditorGUILayout.EndVertical();
		}

		private void CreateEntryUnder(int entryIndexCalled)
		{
			var newEntry = CreateDialogueEntryInstance(false);

			var currentEntriesCount = _target.DialogueEntries.Count;
			if (entryIndexCalled >= currentEntriesCount)
			{
				return;
			}

			//Пройти по всем от последней до текущей и поменять местами
			for (int i = currentEntriesCount - 1; i > entryIndexCalled; i--)
			{
				MoveEntry(i, i - 1);
			}
		}

		private void MoveEntry(int oldIndex, int newIndex)
		{
			var entries = _target.DialogueEntries;

			if (newIndex < 0 || newIndex >= entries.Count)
				return;
			foreach (var entry in entries)
			{
				if (entry.ID == oldIndex)
				{
					foreach (var nextEntriesLink in entry.NextEntriesLinks.Where(nextEntriesLink =>
						         nextEntriesLink.TargetEntryID == newIndex))
					{
						nextEntriesLink.TargetEntryID = oldIndex;
					}

					continue;
				}

				foreach (var link in entry.NextEntriesLinks)
				{
					//все кто ссылались на target элемент - теперь ссылаются на его новую позицию
					if (link.TargetEntryID.Equals(newIndex))
					{
						link.TargetEntryID = oldIndex;
					}
					//все кто ссылались на start элемент - теперь ссылаются на его новую позицию
					else if (link.TargetEntryID.Equals(oldIndex))
					{
						link.TargetEntryID = newIndex;
					}
				}
			}

			(entries[oldIndex].ID, entries[newIndex].ID) = (entries[newIndex].ID, entries[oldIndex].ID);
			(entries[oldIndex], entries[newIndex]) = (entries[newIndex], entries[oldIndex]);
		}

		#region Draw Entry Links

		private void DrawEntryLinks(DialogueEntryConfig entry, int index)
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add Link", EditorStyles.boldLabel);
			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				entry.NextEntriesLinks.Add(new DialogueLink { });
			}

			EditorGUILayout.EndHorizontal();

			EditorGUI.indentLevel++;

			bool flag = entry.NextEntriesLinks.Count > 1;
			for (int j = 0; j < entry.NextEntriesLinks.Count; j++)
			{
				var link = entry.NextEntriesLinks[j];
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();


				link.TargetEntryID = DrawEntryDropdown(link.TargetEntryID, index);
				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					entry.NextEntriesLinks.RemoveAt(j);
					break;
				}

				EditorGUILayout.EndHorizontal();
				if (flag)
				{
					link.ButtonText = EditorGUILayout.TextField(CreateTooltip("Button token", "Токен текста на кнопке"),
						link.ButtonText);
				}

				EditorGUILayout.EndVertical();
			}

			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
		}

		private int DrawEntryDropdown(int currentIndex, int selfIndex)
		{
			var ids = _target.DialogueEntries
				.Select((e, i) => e.Name)
				.Where((id, i) => i != selfIndex) // исключаем сам себя
				.ToArray();

			var indices = _target.DialogueEntries
				.Select((e, i) => i)
				.Where(i => i != selfIndex)
				.ToArray();
			if (ids.Length == 0)
				return 0;

			int selected = Mathf.Max(0, System.Array.IndexOf(indices, currentIndex));
			selected = EditorGUILayout.Popup("Target Entry", selected, ids);

			return indices[selected];
		}

		#endregion

		#region Remove Entry

		private void RemoveEntry(int index)
		{
			var entries = _target.DialogueEntries;
			for (var i = 0; i < entries.Count; i++)
			{
				if (index == i)
					continue;
				var entry = entries[i];

				for (var j = 0; j < entry.NextEntriesLinks.Count; j++)
				{
					var entriesLink = entry.NextEntriesLinks[j];
					if (!entriesLink.TargetEntryID.Equals(index))
						continue;
					entry.NextEntriesLinks.Remove(entriesLink);
				}
			}

			_target.DialogueEntries.RemoveAt(index);

			RecalculateIDs();
		}

		private void RecalculateIDs()
		{
			var entries = _target.DialogueEntries;
			for (int i = 0; i < entries.Count; i++)
			{
				var entry = entries[i];
				if (entry.ID.Equals(i))
					continue;
				ChangeLinkTargetID(entry.ID, i);
				entry.ID = i;
			}
		}

		private void ChangeLinkTargetID(int oldID, int newID)
		{
			var entries = _target.DialogueEntries;
			foreach (var entry in entries)
			{
				if (entry.ID.Equals(oldID))
					continue;
				foreach (var link in entry.NextEntriesLinks)
				{
					if (link.TargetEntryID.Equals(oldID))
					{
						link.TargetEntryID = newID;
					}
				}
			}
		}

		#endregion

		#region EntryEvents

		private void DrawEntryEvents(DialogueEntryConfig entry, int index)
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Add Action", EditorStyles.boldLabel);

			if (GUILayout.Button("+", GUILayout.Width(20)))
			{
				entry.EndEvents.Add(new CustomAction());
				return;
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

			for (var i = 0; i < entry.EndEvents.Count; i++)
			{
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginHorizontal();
				var entryEndEvent = entry.EndEvents[i];
				DrawEntryEvent(entryEndEvent, index, i);

				if (GUILayout.Button("X", GUILayout.Width(20)))
				{
					entry.EndEvents.Remove(entryEndEvent);
					return;
				}

				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.EndVertical();
		}

		private void DrawEntryEvent(CustomAction entryEndEvent, int entryIndex, int eventIndex)
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField($"Action {eventIndex + 1}", EditorStyles.miniBoldLabel);
			serializedObject.Update();
			var entriesProp = serializedObject.FindProperty("DialogueEntries");
			var entryProp = entriesProp.GetArrayElementAtIndex(entryIndex);
			var eventsProp = entryProp.FindPropertyRelative("EndEvents");
			var eventProp = eventsProp.GetArrayElementAtIndex(eventIndex);

			EditorGUILayout.PropertyField(eventProp, GUIContent.none, true);
			serializedObject.ApplyModifiedProperties();
			GUILayout.EndVertical();
		}

		private GUIContent CreateTooltip(string value, string tooltip)
		{
			return new GUIContent(value, tooltip);
		}

		#endregion
	}
}