using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/*
[CustomPropertyDrawer(typeof(ClothingPiece))]
public class ClothingPiecePropertyDrawer : PropertyDrawer
{
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // The 6 comes from extra spacing between the fields (2px each)
        return EditorGUIUtility.singleLineHeight * 4 + 6;
    }
    

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.LabelField(position, label);

        var tagRect = new Rect(position.x, position.y + 18, position.width, 16);
        var displayNameRect = new Rect(position.x, position.y + 36, position.width, 16);

        EditorGUI.indentLevel++;

        EditorGUI.PropertyField(tagRect, property.FindPropertyRelative("Tag"));
        EditorGUI.PropertyField(displayNameRect, property.FindPropertyRelative("DisplayName"));

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }
}*/