using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExtensionsEditor {
	public static void DrawIVector2(string iVName, SerializedObject sO) {
		SerializedProperty sP = sO.FindProperty(iVName);
		NGUIEditorTools.DrawProperty("X", sP.FindPropertyRelative("x"), true, GUILayout.MinWidth(20f));
		NGUIEditorTools.DrawProperty("Y", sP.FindPropertyRelative("y"), true, GUILayout.MinWidth(20f));
	}
	public static void DrawIVector3(string iVName, SerializedObject sO) {
		SerializedProperty sP = sO.FindProperty(iVName);
		NGUIEditorTools.DrawProperty("X", sP.FindPropertyRelative("x"), true, GUILayout.MinWidth(20f));
		NGUIEditorTools.DrawProperty("Y", sP.FindPropertyRelative("y"), true, GUILayout.MinWidth(20f));
		NGUIEditorTools.DrawProperty("Z", sP.FindPropertyRelative("z"), true, GUILayout.MinWidth(20f));
	}
}
