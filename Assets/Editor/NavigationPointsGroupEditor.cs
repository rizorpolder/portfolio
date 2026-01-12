#if UNITY_EDITOR
using Game.Navigation;
using Game.Scripts.Systems.Navigation;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(NavigationPointsGroup))]
	public class NavigationPointsGroupEditor : UnityEditor.Editor
	{
		private NavigationPointsGroup _navigationPoints => target as NavigationPointsGroup;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUI.changed)
				EditorUtility.SetDirty(_navigationPoints);
		}

		private void OnEnable()
		{
			SceneView.duringSceneGui += OnSceneGUI;
		}

		private void AddItem(Vector2 pos)
		{
			serializedObject.ApplyModifiedProperties();

			var point = new NavigationPoint
			{
				Name = "point_" + _navigationPoints.Count(),
				Position = pos,
			};

			int c = _navigationPoints.Count();
			while (_navigationPoints.Contain(point.Name))
			{
				point.Name = "point_" + (++c).ToString();
			}

			_navigationPoints.Add(point);

			serializedObject.Update();
			EditorUtility.SetDirty(target);
		}

		private void OnDisable()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
		}

		private void OnSceneGUI(SceneView view)
		{
			var style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = _navigationPoints.TextColor;

			foreach (var point in _navigationPoints.Points)
			{
				var oldPos = point.Position;

				Handles.Label(oldPos, point.Name, style);
				var fmh_62_52_638330650323511701 = Quaternion.identity;
				Vector2 newPos = Handles.FreeMoveHandle(oldPos, .1f, Vector2.zero, Handles.CylinderHandleCap);
				if (newPos != (Vector2) oldPos)
				{
					point.Position = newPos;
					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();
					EditorUtility.SetDirty(target);
				}
			}

			Event guiEvent = Event.current;
			Vector2 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

			if (guiEvent.type == EventType.MouseUp && guiEvent.button == 1)
			{
				AddItem(mousePosition);
			}

			if (guiEvent.type == EventType.KeyUp && guiEvent.keyCode == KeyCode.Delete)
			{
				float minDstToAnchor = 0.5f;
				NavigationPoint pointToDelete = null;

				foreach (var point in _navigationPoints.Points)
				{
					float dst = Vector2.Distance(mousePosition, point.Position);
					if (dst < minDstToAnchor)
					{
						minDstToAnchor = dst;
						pointToDelete = point;
					}
				}

				if (pointToDelete != null)
				{
					serializedObject.ApplyModifiedProperties();
					_navigationPoints.Remove(pointToDelete);
					serializedObject.Update();
					EditorUtility.SetDirty(target);
				}
			}
		}
	}
}
#endif