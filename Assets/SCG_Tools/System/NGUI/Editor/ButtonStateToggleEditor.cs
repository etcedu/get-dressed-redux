using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

#if UNITY_3_5
[CustomEditor(typeof(PauseToggle))]
#else
[CustomEditor(typeof(ButtonStateToggle), true)]
#endif
public class ButtonStateToggleEditor : Editor {
	static public ButtonStateToggleEditor instance;
	ButtonStateToggle bst;
	
	protected virtual void OnEnable() {
		instance = this;
		bst = (ButtonStateToggle)serializedObject.targetObject;
	}
	protected virtual void OnDisable() {
		instance = null;
		bst = null;
	}
	
	public override void OnInspectorGUI() {
		EditorGUILayout.Space();
		
		serializedObject.Update();
		
		EditorGUI.BeginDisabledGroup(!ShouldDrawProperties());
		DrawCustomProperties();
		EditorGUI.EndDisabledGroup();
		DrawFinalProperties();
		
		serializedObject.ApplyModifiedProperties();
	}
	
	protected virtual bool ShouldDrawProperties() {
		return true;
	}
	protected virtual void DrawCustomProperties() { }
	
	protected virtual void DrawFinalProperties() {
		DrawStateToggleInfo();
	}
	protected virtual void OnDrawFinalProperties() { }
	
	
	public void DrawStateToggleInfo() {
		if(bst == null) return;
		NGUIEditorTools.SetLabelWidth(120);
		NGUIEditorTools.DrawProperty("Code Click Execute", serializedObject.FindProperty("codeClickExecute"));
		NGUIEditorTools.SetLabelWidth(76);
		ButtonStateEditor.Draw("Starting State - "+bst._startingState.state,"_startingState", serializedObject, bst._startingState.onClickEvents);
		if(GUILayout.Button("Copy Colors - To Button"))
		{
			Undo.RecordObject(bst, "Color Copy");
			CopyColors(bst._startingState);
		}
		if(GUILayout.Button("Copy Colors - To Toggle"))
		{
			Undo.RecordObject(bst, "Color Copy");
			CopyColors(bst._startingState, bst._toggleState);
		}
		if(GUILayout.Button("Copy Sprites - To Toggle"))
		{
			Undo.RecordObject(bst, "Sprite Copy");
			CopySprites(bst._startingState, bst._toggleState);
		}
		ButtonStateEditor.Draw("Toggle State - "+bst._toggleState.state,"_toggleState", serializedObject, bst._toggleState.onClickEvents);
		if(GUILayout.Button("Copy Colors - To Button"))
		{
			Undo.RecordObject(bst, "Color Copy");
			CopyColors(bst._toggleState);
		}
		if(GUILayout.Button("Copy Colors - To Start"))
		{
			Undo.RecordObject(bst, "Color Copy");
			CopyColors(bst._toggleState, bst._startingState);
		}
		if(GUILayout.Button("Copy Sprites - To Start"))
		{
			Undo.RecordObject(bst, "Sprite Copy");
			CopySprites(bst._toggleState, bst._startingState);
		}
	}
	
	void CopyColors(ButtonState origin)
	{
		UIButton button = bst.GetComponent<UIButton>();
		button.defaultColor = origin.colorNormal;
		button.hover = origin.colorHover;
		button.pressed = origin.colorPressed;
		button.disabledColor = origin.colorDisabled;
	}
	void CopyColors(ButtonState origin, ButtonState target)
	{
		target.colorNormal = origin.colorNormal;
		target.colorHover = origin.colorHover;
		target.colorPressed = origin.colorPressed;
		target.colorDisabled = origin.colorDisabled;
	}

	void CopySprites(ButtonState origin, ButtonState target)
	{
		target.spriteNormal = origin.spriteNormal;
		target.sprite2D_Normal = origin.sprite2D_Normal;
		target.spriteHover = origin.spriteHover;
		target.sprite2D_Hover = origin.sprite2D_Hover;
		target.spritePressed = origin.spritePressed;
		target.sprite2D_Pressed = origin.sprite2D_Pressed;
		target.spriteDisabled = origin.spriteDisabled;
		target.sprite2D_Disabled = origin.sprite2D_Disabled;
	}
}

