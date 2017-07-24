using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildHandler))]
public class BuildHandlerEditor : Editor {
	
	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI();


		BuildHandler buildHandler = (BuildHandler) target;

		buildHandler.GridShowMod = (BuildHandler.BuildGridShowMod)EditorGUILayout.EnumPopup("Grid show mod", buildHandler.GridShowMod);
		if (buildHandler.GridShowMod == BuildHandler.BuildGridShowMod.Gizmos)
		{
			buildHandler.GizmosGridColor = EditorGUILayout.ColorField("Gizmos color", buildHandler.GizmosGridColor);
		}

		buildHandler.UseSecureSizes = EditorGUILayout.Toggle("Use secure size", buildHandler.UseSecureSizes);

		buildHandler.SpacingCountX = EditorGUILayout.IntField("Spacing count X", buildHandler.SpacingCountX);
		buildHandler.SpacingCountY = EditorGUILayout.IntField("Spacing count Y", buildHandler.SpacingCountY);

	}
}
