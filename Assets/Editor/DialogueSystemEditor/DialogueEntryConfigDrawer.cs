using Game.Scripts.Systems.DialogueSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.DialogueSystemEditor
{
	[CustomPropertyDrawer(typeof(DialogueEntryConfig))]
	public class DialogueEntryConfigDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Обертка
			Rect boxRect = new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label));
			GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

			Rect contentRect = new Rect(position.x + 5,
				position.y + 5,
				position.width - 10,
				EditorGUIUtility.singleLineHeight);
			float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

			// Основные поля
			EditorGUI.LabelField(contentRect,
				$"Entry ID: {property.FindPropertyRelative("ID").intValue}",
				EditorStyles.boldLabel);
			contentRect.y += lineHeight;

			DrawField(property, "Name", "Entry name", ref contentRect, lineHeight);
			DrawField(property, "SpeakerID", "Speaker ID", ref contentRect, lineHeight);
			DrawField(property, "ListenerID", "Listener ID", ref contentRect, lineHeight);
			DrawField(property, "DialogueTextToken", "Text Token", ref contentRect, lineHeight);

			//Отрисовка списка NextEntriesLink
			var linksProp = property.FindPropertyRelative("NextEntriesLinks");
			EditorGUI.LabelField(contentRect, "Next Entries Link", EditorStyles.boldLabel);
			contentRect.y += lineHeight;


			for (int i = 0; i < linksProp.arraySize; i++)
			{
				var linkProp = linksProp.GetArrayElementAtIndex(i);
				EditorGUI.indentLevel++;

				// Горизонтальная линия: TargetEntryID + кнопка X
				Rect rowRect = new Rect(contentRect.x,
					contentRect.y,
					contentRect.width,
					EditorGUIUtility.singleLineHeight);
				Rect targetRect = new Rect(rowRect.x, rowRect.y, rowRect.width - 25, rowRect.height);
				Rect buttonRect = new Rect(rowRect.x + rowRect.width - 20, rowRect.y, 20, rowRect.height);

				EditorGUI.PropertyField(targetRect,
					linkProp.FindPropertyRelative("TargetEntryID"),
					new GUIContent("Target ID"));

				if (GUI.Button(buttonRect, "X"))
				{
					linksProp.DeleteArrayElementAtIndex(i);
					property.serializedObject.ApplyModifiedProperties();
					break;
				}

				contentRect.y += lineHeight;

				// ButtonText под TargetEntryID
				DrawField(linkProp, "ButtonText", "Button Text", ref contentRect, lineHeight);

				contentRect.y += 2;
				EditorGUI.indentLevel--;
			}

			// Кнопка добавления
			Rect addRect = new Rect(contentRect.x, contentRect.y, 120, EditorGUIUtility.singleLineHeight);
			if (GUI.Button(addRect, "Добавить ссылку"))
			{
				linksProp.arraySize++;
				property.serializedObject.ApplyModifiedProperties();
			}

			EditorGUI.EndProperty();
		}

		private void DrawField(SerializedProperty parent,
			string fieldName,
			string label,
			ref Rect rect,
			float lineHeight)
		{
			var prop = parent.FindPropertyRelative(fieldName);
			EditorGUI.PropertyField(rect, prop, new GUIContent(label));
			rect.y += lineHeight;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			int baseLines = 5; // ID, Name, SpeakerID, ListenerID, TextToken
			int linkLines = 1; // заголовок
			var linksProp = property.FindPropertyRelative("NextEntriesLinks");
			linkLines += linksProp.arraySize * 3; // TargetID + ButtonText + кнопка удаления

			return (baseLines + linkLines + 1) *
				(EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) + 10;
		}
	}
}