using SimcoachGames.EventRecorder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildVersionToggleSettingsSO))]
public class BuildVersionToggleSettingsSOInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        base.OnInspectorGUI();
        GUI.enabled = true;

        EditorGUILayout.HelpBox("Use the [Tools/Event Recorder/Consumer or Enterprise Version Toggle] window to edit this file",
            MessageType.Info);
        if (GUILayout.Button("Open Editor Window"))
            VersionBuildToggleWindow.ShowWindow();
    }
}
