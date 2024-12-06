//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------


/*-------------------*
 * Edited for Skill Arcade portable use by Garrett Kimball  
 * ------------------*/

using UnityEngine;
using UnityEditor;

namespace Simcoach.SkillArcade
{
    [CustomEditor(typeof(TweenPosition_SA))]
    public class TweenPositionEditor_SA : UITweenerEditor_SA
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            //NGUIEditorTools.SetLabelWidth(120f);
            EditorGUIUtility.labelWidth = 120f;

            TweenPosition_SA tw = target as TweenPosition_SA;
            GUI.changed = false;

            Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
            Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
            bool ignoreEqualFields = EditorGUILayout.Toggle("Ignore Equal Fields", tw.ignoreEqualFields);

            if (GUI.changed)
            {
                Undo.RecordObject(tw, "Tween Change");
                //NGUIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                tw.ignoreEqualFields = ignoreEqualFields;
                //NGUITools.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }

}