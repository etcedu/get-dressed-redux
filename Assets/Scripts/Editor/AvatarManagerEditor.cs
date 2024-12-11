using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor (typeof(AvatarManager))]
//[CanEditMultipleObjects]
public class AvatarManagerEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		GUILayout.Space(10);
		CustomEditorTools.SetLabelWidth(100);
		NGUIEditorTools.DrawProperty("Char. Animator", serializedObject.FindProperty("characterAnimator"));
		
		GUILayout.Space(10);
		if(CustomEditorTools.DrawHeader("Male", serializedObject.ToString() + "_Male"))
		{
			CustomEditorTools.BeginContents();
			{
				NGUIEditorTools.DrawProperty("Controller", serializedObject.FindProperty("maleController"));
				NGUIEditorTools.DrawProperty("Toggle", serializedObject.FindProperty("maleToggle"));
				serializedObject.DrawArray("Setup", "maleSetup");
			} CustomEditorTools.EndContents();
		}
		GUILayout.Space(10);
		
		if(CustomEditorTools.DrawHeader("Female", serializedObject.ToString() + "_Female"))
		{
			CustomEditorTools.BeginContents();
			{
				NGUIEditorTools.DrawProperty("Controller", serializedObject.FindProperty("femaleController"));
				NGUIEditorTools.DrawProperty("Toggle", serializedObject.FindProperty("femaleToggle"));
				serializedObject.DrawArray("Setup", "femaleSetup");
			} CustomEditorTools.EndContents();
		}
		GUILayout.Space(10);
		
        serializedObject.DrawArray("Colors", "colorToggles");

        serializedObject.ApplyModifiedProperties();
	}
}
