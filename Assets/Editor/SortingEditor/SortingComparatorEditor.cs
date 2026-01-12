using Game.Scripts.Systems.Sorting;
using UnityEditor;
using UnityEngine;

namespace Editor.Sorting
{
	[CustomEditor(typeof(SortingComparator))]
	public class SortingComparatorEditor : UnityEditor.Editor
	{
		private SortingComparator comparator => target as SortingComparator;
		private bool editMode = false;

		public override void OnInspectorGUI()
		{
			comparator.sortingType =
				(SortingComparator.SortingType) EditorGUILayout.EnumPopup("Type", comparator.sortingType);

			comparator.Is2DObject = EditorGUILayout.Toggle("Is 2D object", comparator.Is2DObject);
			if (comparator.Is2DObject)
			{
				EditorGUI.indentLevel++;
				comparator.sortingOrderLower =
					EditorGUILayout.IntField("Sorting order lower", comparator.sortingOrderLower);
				comparator.sortingOrderUpper =
					EditorGUILayout.IntField("Sorting order upper", comparator.sortingOrderUpper);
				EditorGUI.indentLevel--;
			}

			if (comparator.points.Count == 0)
				comparator.points.Add(Vector3.zero);

			if ((comparator.points.Count == 1 && comparator.sortingType == SortingComparator.SortingType.Line))
				comparator.points.Add(Vector3.up);


			if (!editMode)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Enable edit mode");

				if (GUILayout.Button("+"))
				{
					EnableEditMode();
				}

				GUILayout.EndHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Disable edit mode");

				if (GUILayout.Button("-"))
				{
					DisableEditMode();
				}

				GUILayout.EndHorizontal();
			}

			EditorUtility.SetDirty(comparator);
		}

		private void EnableEditMode()
		{
			editMode = true;
		}

		private void DisableEditMode()
		{
			editMode = false;
		}

		private void OnSceneGUI()
		{
			if (!editMode)
				return;

			if (comparator.sortingType == SortingComparator.SortingType.Line)
			{
				Handles.DrawLine(comparator.points[0] + comparator.transform.position,
					comparator.points[1] + comparator.transform.position);
			}

			for (int i = 0; i < comparator.points.Count; ++i)
			{
				var oldPos = comparator.points[i] + comparator.transform.position;
				var fmh_86_6_638330650323613293 = Quaternion.identity;
				Vector2 newPos = Handles.FreeMoveHandle(oldPos,
					.1f,
					Vector2.zero,
					Handles.CylinderHandleCap);
				if (newPos != (Vector2) oldPos)
				{
					Undo.RecordObject(comparator, "Move point");
					comparator.MovePoint(i, newPos - (Vector2) comparator.transform.position);
				}
			}
		}
	}
}