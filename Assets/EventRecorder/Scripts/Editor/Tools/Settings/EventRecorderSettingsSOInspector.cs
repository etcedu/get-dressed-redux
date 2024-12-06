using UnityEditor;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    [CustomEditor(typeof(EventRecorderSettingsSO))]
    public class EventRecorderSettingsSOInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;

            EditorGUILayout.HelpBox("Use the [Tools/Event Recorder/Settings] window to edit this file",
                MessageType.Info);
            if (GUILayout.Button("Open Editor Window"))
                EventRecorderSettingsWindow.Init();
        }
    }
}