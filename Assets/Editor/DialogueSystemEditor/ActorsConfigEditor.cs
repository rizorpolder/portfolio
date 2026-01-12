using System;
using Game.Scripts.Systems.DialogueSystem;
using UnityEditor;
using UnityEngine;

namespace Editor.DialogueSystemEditor
{
	[CustomEditor(typeof(ActorsConfig))]
	public class ActorsConfigEditor : UnityEditor.Editor
	{
		private ActorsConfig _target;

		private void OnEnable()
		{
			_target = target as ActorsConfig;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Actor ID", EditorStyles.boldLabel);

			var entriesList = serializedObject.FindProperty("Actors");
			entriesList.isExpanded = EditorGUILayout.Foldout(entriesList.isExpanded, "Actors");

			if (entriesList.isExpanded)
			{
				for (int i = 0; i < _target.Actors.Count; i++)
				{
					DrawActorEntry(_target.Actors[i], i);
				}

				if (GUILayout.Button("Добавить"))
				{
					AddActorInstance();
				}
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(_target);
			}
		}

		private void DrawActorEntry(ActorConfig targetActor, int index)
		{
			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField($"Actor ID: {targetActor.ID}", EditorStyles.miniBoldLabel);
			if (GUILayout.Button("X", GUILayout.Width(20)))
			{
				RemoveEntry(targetActor);
				return;
			}

			EditorGUILayout.EndHorizontal();
			targetActor.IsActive = EditorGUILayout.Toggle("Is Active", targetActor.IsActive);
			targetActor.ActorName = EditorGUILayout.TextField("Actor Name", targetActor.ActorName);
			targetActor.IsPlayer = EditorGUILayout.Toggle("Is Player", targetActor.IsPlayer);
			targetActor.InitialPointName =
				EditorGUILayout.TextField("Initial Point Name", targetActor.InitialPointName);
			targetActor.InitialPositionVect =
				EditorGUILayout.Vector3Field("Initial Position", targetActor.InitialPositionVect);
			targetActor.Icon = EditorGUILayout.ObjectField("Icon", targetActor.Icon, typeof(Sprite), false) as Sprite;
			
			
			
			var entriesProp = serializedObject.FindProperty("Actors");
			var entryProp = entriesProp.GetArrayElementAtIndex(index);
			EditorGUILayout.PropertyField(entryProp.FindPropertyRelative("AssetRef"));
			
			
			EditorGUILayout.EndVertical();
		}

		private void RemoveEntry(ActorConfig targetActor)
		{
			_target.Actors.Remove(targetActor);
		}

		private void AddActorInstance()
		{
			var entryInstance = new ActorConfig
			{
				ID = _target.Actors.Count
			};

			_target.Actors.Add(entryInstance);
		}
	}
}