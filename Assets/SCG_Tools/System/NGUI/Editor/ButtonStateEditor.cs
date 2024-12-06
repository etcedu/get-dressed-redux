using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ButtonStateEditor : Editor {
	public static void Draw(string header, string state, SerializedObject sO, List<EventDelegate> onClickEvents) {
		if(NGUIEditorTools.DrawHeader(header)) {
			SerializedProperty _state = sO.FindProperty(state);
			NGUIEditorTools.BeginContents();
			NGUIEditorTools.DrawProperty("Name", _state.FindPropertyRelative("state"), false);
			NGUIEditorTools.DrawProperty("Target", _state.FindPropertyRelative("button"), false);
			if(_state.FindPropertyRelative("button") != null && _state.FindPropertyRelative("button").objectReferenceValue != null) {
				UIButton btn = (_state.FindPropertyRelative("button").objectReferenceValue as GameObject).GetComponent<UIButton>();
				if(btn != null && btn.tweenTarget != null) {
					DrawColors(_state, state);
					DrawSprites(_state, state, btn, sO);
				}
			}
			if(!sO.isEditingMultipleObjects) 
				NGUIEditorTools.DrawEvents("On Click - "+_state.FindPropertyRelative("state").stringValue, sO.targetObject, onClickEvents, false);
			NGUIEditorTools.EndContents();
		}
	}
	
	protected static void DrawColors (SerializedProperty state, string stateName) {
		if (NGUIEditorTools.DrawHeader("Colors", stateName+"_Colors", false, true))
		{
			CustomEditorTools.StartIndent(20); {
				GUILayout.BeginVertical();
				NGUIEditorTools.DrawProperty("Normal", state.FindPropertyRelative("colorNormal"), false);
				NGUIEditorTools.DrawProperty("Hover", state.FindPropertyRelative("colorHover"), false);
				NGUIEditorTools.DrawProperty("Pressed", state.FindPropertyRelative("colorPressed"), false);
				NGUIEditorTools.DrawProperty("Disabled", state.FindPropertyRelative("colorDisabled"), false);
				GUILayout.EndVertical();
			} CustomEditorTools.EndIndent();
		}
	}
	
	protected static void DrawSprites(SerializedProperty state, string stateName, UIButton btn, SerializedObject sO) {
		UISprite sprite = btn.tweenTarget.GetComponent<UISprite>();
		if (sprite != null)
		{
			if (NGUIEditorTools.DrawHeader("Sprites", stateName+"Sprites", false, true))
			{
				CustomEditorTools.StartIndent(20); {
					EditorGUI.BeginDisabledGroup(sO.isEditingMultipleObjects);
					{
						SerializedProperty atlas = (new SerializedObject(sprite)).FindProperty("mAtlas");
						
						GUILayout.BeginVertical();
						NGUIEditorTools.DrawSpriteField("Normal", sO, atlas, state.FindPropertyRelative("spriteNormal"));
						NGUIEditorTools.DrawSpriteField("Hover", sO, atlas, state.FindPropertyRelative("spriteHover"), true);
						NGUIEditorTools.DrawSpriteField("Pressed", sO, atlas, state.FindPropertyRelative("spritePressed"), true);
						NGUIEditorTools.DrawSpriteField("Disabled", sO, atlas, state.FindPropertyRelative("spriteDisabled"), true);
						GUILayout.EndVertical();
					}
					EditorGUI.EndDisabledGroup();
				} CustomEditorTools.EndIndent();
			}
		} else
		{
			UI2DSprite sprite2d = btn.tweenTarget.GetComponent<UI2DSprite>();
			if(sprite2d != null)
			{
				if (NGUIEditorTools.DrawHeader("Sprites", state+"Sprites", false, true))
				{
					CustomEditorTools.StartIndent(20); {
						EditorGUI.BeginDisabledGroup(sO.isEditingMultipleObjects);
						{
							GUILayout.BeginVertical();
							NGUIEditorTools.DrawProperty("Normal", state.FindPropertyRelative("sprite2D_Normal"));
							NGUIEditorTools.DrawProperty("Hover", state.FindPropertyRelative("sprite2D_Hover"));
							NGUIEditorTools.DrawProperty("Pressed", state.FindPropertyRelative("sprite2D_Pressed"));
							NGUIEditorTools.DrawProperty("Disabled", state.FindPropertyRelative("sprite2D_Disabled"));
							GUILayout.EndVertical();
						}
						EditorGUI.EndDisabledGroup();
					} CustomEditorTools.EndIndent();
				}
			}
		}
	}
}