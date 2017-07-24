using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Build))]
public class BuildEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		Build build = (Build)target;

		build.ApplyBuildHandlerScale = EditorGUILayout.Toggle("Apply BuildHandler scale", build.ApplyBuildHandlerScale);
		build.UseRandomColor = EditorGUILayout.Toggle("Use random color", build.UseRandomColor);

		build.Size = EditorGUILayout.Vector3Field("Build collider size", build.Size);

	}
}
