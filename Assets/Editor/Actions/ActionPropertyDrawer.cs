using System;
using Game.Scripts.Systems.DialogueSystem.Data;
using UnityEditor;
using UnityEngine;

namespace Editor.Actions
{
	[CustomPropertyDrawer(typeof(CustomAction))]
	public class DialogueEntryEventProp : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = 0f;
			float y = position.y;

			// Название переменной — label.text
			Rect labelRect = new Rect(position.x, y, position.width, lineHeight);
			GUIStyle noIndentLabel = new GUIStyle(EditorStyles.label);
			noIndentLabel.padding.left = -10;
			noIndentLabel.margin.left = -10;

// Отрисовка без отступа
			EditorGUI.LabelField(labelRect, label.text, noIndentLabel);
			y += lineHeight + spacing;

			// ActionType enum
			SerializedProperty actionTypeProp = property.FindPropertyRelative("ActionType");
			Rect enumRect = new Rect(position.x, y, position.width, lineHeight);
			EditorGUI.PropertyField(enumRect, actionTypeProp);
			y += lineHeight + spacing;

			// ActionData
			SerializedProperty actionDataProp = property.FindPropertyRelative("ActionData");
			if (actionTypeProp.enumValueIndex < 0)
			{
				actionTypeProp.enumValueIndex = 0;
			}

			ActionType selectedType = (ActionType) actionTypeProp.enumValueIndex;

			Type expectedType = CustomActionTypeMap.DataTypes[selectedType];


			if (actionDataProp.managedReferenceValue == null ||
			    actionDataProp.managedReferenceValue.GetType() != expectedType)
			{
				actionDataProp.managedReferenceValue = Activator.CreateInstance(expectedType);
			}


			float dataHeight = EditorGUI.GetPropertyHeight(actionDataProp, true);
			Rect dataRect = new Rect(position.x, y, position.width, dataHeight);

			EditorGUI.PropertyField(dataRect, actionDataProp, true);

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty actionDataProp = property.FindPropertyRelative("ActionData");

			float labelHeight = EditorGUIUtility.singleLineHeight;
			float enumHeight = EditorGUIUtility.singleLineHeight;
			float spacing = 2f;

			float dataHeight = EditorGUI.GetPropertyHeight(actionDataProp, true);

			return labelHeight + spacing + enumHeight + spacing + dataHeight;
		}
	}
}