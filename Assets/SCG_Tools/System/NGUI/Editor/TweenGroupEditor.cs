using UnityEngine;
using UnityEditor;
using System.Collections;

public class TweenGroupEditor {

    public static void Draw(SerializedProperty sP, ref Vector2 scrollPos)
    {
        GUILayout.BeginVertical();
        {
            SerializedProperty targets = sP.FindPropertyRelative("targets");
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(70), GUILayout.Height(Mathf.Min(50f + (20f * Mathf.Max(targets.arraySize, 1)), 350f)));
            {
                if (targets.arraySize == 0)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
                        {
                            targets.InsertArrayElementAtIndex(0);
                        }
                    } GUILayout.EndHorizontal();
                }
                else
                {
                    for (int i = 0; i < targets.arraySize; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
                            {
                                targets.InsertArrayElementAtIndex(i);
                                GUILayout.EndHorizontal();
                                break;
                            }
                            NGUIEditorTools.DrawProperty("Tween " + (i+1), targets.GetArrayElementAtIndex(i), true, GUILayout.MinWidth(20f));
                            if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
                            {
                                targets.DeleteArrayElementAtIndex(i);
                                GUILayout.EndHorizontal();
                                break;
                            }
                            if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
                            {
                                targets.InsertArrayElementAtIndex(i + 1);
                            }
                        } GUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(10);
                NGUIEditorTools.DrawProperty("Group #", sP.FindPropertyRelative("group"), true, GUILayout.MinWidth(20f));
                NGUIEditorTools.DrawProperty("Direction", sP.FindPropertyRelative("direction"), true, GUILayout.MinWidth(20f));
            } GUILayout.EndScrollView();
        } GUILayout.EndVertical();
    }
}

public class ListEditor {
	
	public static void Draw(SerializedProperty sP, ref Vector2 scrollPos)
	{
		GUILayout.BeginVertical();
		{
			SerializedProperty targets = sP.FindPropertyRelative("targets");
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(70), GUILayout.Height(Mathf.Min(50f + (20f * Mathf.Max(targets.arraySize, 1)), 350f)));
			{
				if (targets.arraySize == 0)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();
						if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
						{
							targets.InsertArrayElementAtIndex(0);
						}
					} GUILayout.EndHorizontal();
				}
				else
				{
					for (int i = 0; i < targets.arraySize; ++i)
					{
						GUILayout.BeginHorizontal();
						{
							if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
							{
								targets.InsertArrayElementAtIndex(i);
								GUILayout.EndHorizontal();
								break;
							}
							NGUIEditorTools.DrawProperty("Tween " + (i+1), targets.GetArrayElementAtIndex(i), true, GUILayout.MinWidth(20f));
							if (GUILayout.Button("-", GUILayout.MaxWidth(30)))
							{
								targets.DeleteArrayElementAtIndex(i);
								GUILayout.EndHorizontal();
								break;
							}
							if (GUILayout.Button("+", GUILayout.MaxWidth(30)))
							{
								targets.InsertArrayElementAtIndex(i + 1);
							}
						} GUILayout.EndHorizontal();
					}
				}
			} GUILayout.EndScrollView();
		} GUILayout.EndVertical();
	}
}

