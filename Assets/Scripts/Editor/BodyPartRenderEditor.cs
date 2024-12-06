using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer (typeof(ProfessionalDress.BodyPartRender))]
[CustomPropertyDrawer (typeof(AvatarManager.BodyPartRender))]
//[CanEditMultipleObjects]
public class BodyPartRenderEditor : PropertyDrawer 
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
		property.DrawRelativeProperty("Renderer", "renderer");
		property.DrawRelativeProperty("Material", "material");
		//EditorGUI.indentLevel = indent;
		EditorGUILayout.EndVertical();
		CustomEditorTools.EndIndent();
		EditorGUI.EndProperty();
		//}
	}
}
