using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer (typeof(ProfessionalDress.OtherSetup))]
//[CanEditMultipleObjects]
public class OtherSetupEditor : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		//if(Event.current.type == EventType.Repaint)
		//{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		CustomEditorTools.StartIndent(-70, 70);
		EditorGUILayout.BeginVertical();
		//int indent = EditorGUI.indentLevel;
		//EditorGUI.indentLevel = 0;
		property.DrawRelativeProperty("Category", "category");
		property.serializedObject.DrawArray(property, "Clothing", "clothing");
		property.DrawRelativeProperty("Renderer", "renderer");
		property.DrawRelativeProperty("Other", "otherRenderer");
		//EditorGUI.indentLevel = indent;
		EditorGUILayout.EndVertical();
		CustomEditorTools.EndIndent();
		EditorGUI.EndProperty();
		//}
	}
}
