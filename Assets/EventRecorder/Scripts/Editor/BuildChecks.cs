using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class PreBuildCheck : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (EventRecorderSettingsSO.BypassBuildChecks)
                return;

            EventRecorderSettingsSO settings = EventRecorderSettingsSO.GetInstance();

            if (!EventRecorderSettingsSO.IsGameIdValid(settings.eventId))
                throw new BuildFailedException("Event Id is not valid. Please set a valid Event ID in the Event Recorder Settings window.");

            BuildCheckWindow.wasDebugBuild = (report.summary.options & BuildOptions.Development) != 0;

            BuildUseCase useCase = settings.buildUseCase;
            settings.ChangeBuildUseCase(useCase);
            switch (useCase)
            {
                case BuildUseCase.Development:
                    if (!BuildCheckWindow.wasDebugBuild)
                        throw new BuildFailedException(
                            "Development build use case requires the build setting \"Development Build\" be toggled ON");
                    break;

                case BuildUseCase.PlayTest:
                    if (BuildCheckWindow.wasDebugBuild)
                        throw new BuildFailedException(
                            "PlayTest use case requires the build setting \"Development Build\" be toggled OFF");
                    break;

                case BuildUseCase.AppStore:
                    if (BuildCheckWindow.wasDebugBuild)
                        throw new BuildFailedException(
                            "AppStore build use case requires the build setting \"Development Build\" be toggled OFF");
                    break;
            }

            BuildCheckWindow.Init();
        }
    }

    public class PostBuildCheck : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            string buildType = BuildCheckWindow.wasDebugBuild
                ? "<color=magenta>Debug</color>"
                : "<color=red>Production</color>";
            Debug.Log(string.Format("{0} build of <color=cyan>{1}</color> <color=green>v{2}</color> complete",
                buildType,
                PlayerSettings.applicationIdentifier, Application.version));
        }
    }

    public class BuildCheckWindow : EditorWindow
    {
        const float WindowSize = 512;
        static BuildCheckWindow CheckWindow;

        public static bool wasDebugBuild;

        public static void Init()
        {
            CheckWindow = CreateInstance<BuildCheckWindow>();
            CheckWindow.titleContent = new GUIContent("BUILD CHECK");

            CheckWindow.ShowModal();

            float width = (Screen.currentResolution.width / 2) - (WindowSize / 2);
            float height = (Screen.currentResolution.height / 2) - (WindowSize / 2);
            CheckWindow.position = new Rect(width, height, WindowSize, WindowSize);
        }

        void OnGUI()
        {
            EventRecorderSettingsSO settings = EventRecorderSettingsSO.GetInstance();
            
            Color originalColor = GUI.color;
            GUIStyle style = GUI.skin.GetStyle("Label");
            style.alignment = TextAnchor.MiddleCenter;
            style.richText = true;
            
            GUILayout.FlexibleSpace();

            GUILayout.Label("<b>CHECK YOUR BUILD!</b>", style);
            
            GUILayout.Space(20);
            
            GUILayout.Label("Event Recorder Info", style);
            DisplayInfoOnLine("Event Recorder Version: ", EventRecorderSettingsSO.GetEventRecorderVersion());
            GUILayout.Space(5);
            DisplayInfoOnLine("Event Id: ", EventRecorderSettingsSO.GetInstance().eventId, Color.yellow);
            DisplayInfoOnLine("Use Case: ", settings.buildUseCase.ToString(), Color.yellow);
            DisplayInfoOnLine("Endpoint: ", settings.targetEndpoint.endpoint);
            DisplayInfoOnLine("Post Interval: ", settings.postInterval.ToString());
            
            GUILayout.Space(20);
            
            GUILayout.Label("App Info", style);
            DisplayInfoOnLine("Application Identifier: ", PlayerSettings.applicationIdentifier);
            DisplayInfoOnLine("Game Version: ", Application.version);

            GUILayout.Space(20);

            EditorGUILayout.HelpBox(
                "Please check that:" +
                "\n1. Your Event ID is correct" +
                "\n2. Your User Id Mode is correct" +
                "\n3. Your Build Use Case is correct" +
                "\n4. Your Application Identifier is correct" +
                "\n5. You have updated your Game Version",
                MessageType.Info);

            EditorGUILayout.HelpBox(
                "Having all data set correctly is important in order for us to keep the backend data easy to parse. If you have any questions please don't hesitate to ask <3",
                MessageType.Warning);

            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            if (GUILayout.Button("Looks Good"))
            {
                CheckWindow.Close();
            }

            GUI.color = originalColor;
            if (GUILayout.Button("I Made a Mistake"))
            {
                CheckWindow.Close();
                throw new BuildFailedException($"User aborted build");
            }

            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUI.color = originalColor;
        }

        void DisplayInfoOnLine(string label, string value)
        {
            DisplayInfoOnLine(label, value, Color.white);
        }

        void DisplayInfoOnLine(string label, string value, Color color)
        {
            GUIStyle rightAlign = GUI.skin.GetStyle("Label");
            rightAlign.alignment = TextAnchor.MiddleRight;

            Color originalColor = GUI.color;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, rightAlign, GUILayout.Width(200));
            GUI.color = color;
            GUILayout.Label(value);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = originalColor;
        }
    }
}