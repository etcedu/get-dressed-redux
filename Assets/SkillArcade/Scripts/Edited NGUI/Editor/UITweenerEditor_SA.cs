﻿//----------------------------------------------
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

    [CustomEditor(typeof(UITweener_SA), true)]
    public class UITweenerEditor_SA : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            //NGUIEditorTools.SetLabelWidth(110f);
            EditorGUIUtility.labelWidth = 110f;
            base.OnInspectorGUI();
            DrawCommonProperties();
        }

        protected void DrawCommonProperties()
        {
            UITweener_SA tw = target as UITweener_SA;

            if (true)
            {
                //NGUIEditorTools.BeginContents();
                EditorGUIUtility.labelWidth = 110f;

                GUI.changed = false;

                UITweener_SA.Style style = (UITweener_SA.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
                AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
                //UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

                GUILayout.BeginHorizontal();
                float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
                bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

                if (GUI.changed)
                {
                    Undo.RecordObject(tw, "Tween Change");
                    //NGUIEditorTools.RegisterUndo("Tween Change", tw);
                    tw.animationCurve = curve;
                    //tw.method = method;
                    tw.style = style;
                    tw.ignoreTimeScale = ts;
                    tw.tweenGroup = tg;
                    tw.duration = dur;
                    tw.delay = del;
                    //NGUITools.SetDirty(tw);
                }
                //NGUIEditorTools.EndContents();
            }

            //NGUIEditorTools.SetLabelWidth(80f);
            //NGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
        }
    }
}