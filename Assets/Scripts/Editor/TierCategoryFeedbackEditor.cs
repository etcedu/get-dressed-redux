using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer (typeof(ProfessionalDress.TierCategoryFeedback))]
//[CanEditMultipleObjects]
public class TierCategoryFeedbackEditor : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		//if(Event.current.type == EventType.Repaint)
		//{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		CustomEditorTools.StartIndent(-90, 60);
		EditorGUILayout.BeginVertical();
		property.DrawRelativeProperty("Gender", "gender");
		property.serializedObject.DrawArray(property, "Tiers", "tiers");
		property.DrawRelativeProperty("Category", "category");
		property.DrawRelativeProperty("Feedback", "feedback");
		CustomEditorTools.SetLabelWidth(110);
		property.DrawRelativeProperty("Terrible Feedback", "terribleFeedback");
		EditorGUILayout.EndVertical();
		CustomEditorTools.EndIndent();
		EditorGUI.EndProperty();
		//}
	}
}