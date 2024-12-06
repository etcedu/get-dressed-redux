using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer (typeof(Clothing.Info))]
//[CanEditMultipleObjects]
public class ClothingEditor : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		//if(Event.current.type == EventType.Repaint)
		//{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		CustomEditorTools.StartIndent(-65, 70);
		EditorGUILayout.BeginVertical();
		//int indent = EditorGUI.indentLevel;
		//EditorGUI.indentLevel = 0;
		property.DrawRelativeProperty("Tag", "tag");
		property.DrawRelativeProperty("Display", "displayTag");
		property.DrawRelativeProperty("Image", "image");
		property.serializedObject.DrawArray(property, "Pieces", "pieces");
		property.serializedObject.DrawArray(property, "Tiers", "tiers");
		property.DrawRelativeProperty("Gender", "gender");
		property.serializedObject.DrawArray(property, "Colors", "colors");
		property.serializedObject.DrawArray(property, "Matching Colors", "matchingColors");
		property.serializedObject.DrawArray(property, "Designs", "designs");
		property.serializedObject.DrawArray(property, "Matching Designs", "matchingDesigns");
		property.serializedObject.DrawArray(property, "Good Matches", "goodMatches");
		property.serializedObject.DrawArray(property, "Bad Matches", "badMatches");
		//EditorGUI.indentLevel = indent;
		EditorGUILayout.EndVertical();
		CustomEditorTools.EndIndent();
		EditorGUI.EndProperty();
		//}
	}
}
