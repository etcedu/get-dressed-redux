using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomPropertyDrawer (typeof(BitMaskAttribute))]
//[CanEditMultipleObjects]
public class BitMaskAttributeEditor : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
	{
		var typeAttr = attribute as BitMaskAttribute;
		// Add the actual int value behind the field name
		label.text = label.text + "("+property.intValue+")";
		property.intValue = CustomEditorTools.DrawBitMaskField(position, property.intValue, typeAttr.propType, label);
	}
}
