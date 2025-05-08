//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------
//
//   Edited for use with UUI and non-NGUI games
//               Garrett Kimball
//             Simcoach Games 2016
//
//-------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenHeight))]
public class TweenHeightEditor : UITweenerEditor
{
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(120f);

		TweenHeight tw = target as TweenHeight;
		GUI.changed = false;

		int from = EditorGUILayout.IntField("From", tw.from);
		int to = EditorGUILayout.IntField("To", tw.to);

		if (from < 0) from = 0;
		if (to < 0) to = 0;

		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Tween Change", tw);
			tw.from = from;
			tw.to = to;
			NGUITools.SetDirty(tw);
		}

		DrawCommonProperties();
	}
}
