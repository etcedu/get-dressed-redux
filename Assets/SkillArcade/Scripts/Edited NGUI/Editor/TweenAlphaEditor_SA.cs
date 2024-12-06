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
    [CustomEditor(typeof(TweenAlpha_SA))]
    public class TweenAlphaEditor_SA : UITweenerEditor_SA
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            //NGUIEditorTools.SetLabelWidth(120f);
            EditorGUIUtility.labelWidth = 120f;

            TweenAlpha_SA tw = target as TweenAlpha_SA;
            GUI.changed = false;

            float from = EditorGUILayout.Slider("From", tw.from, 0f, 1f);
            float to = EditorGUILayout.Slider("To", tw.to, 0f, 1f);

            if (GUI.changed)
            {
                Undo.RecordObject(tw, "Tween Change");
                //NGUIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                EditorUtility.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }

}