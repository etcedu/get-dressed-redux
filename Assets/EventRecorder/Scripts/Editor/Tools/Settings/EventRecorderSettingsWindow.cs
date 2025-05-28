using System;
using UnityEditor;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderSettingsWindow : EditorWindow
    {
        static EventRecorderSettingsSO Settings;
        static Vector2 ScrollPos;
        static bool RuntimeOptionsToggle = true;
        static bool EditorOptionsToggle = true;
        static bool GameIdHasError = false;
        static string EventRecorderVersion;

        [MenuItem("Tools/Event Recorder/Settings")]
        public static void Init()
        {
            EventRecorderSettingsWindow window =
                (EventRecorderSettingsWindow) GetWindow(typeof(EventRecorderSettingsWindow), false,
                    "Event Recorder Settings");

            Settings = EventRecorderSettingsSO.GetInstance();
            if (!Settings)
                return;

            EventRecorderVersion = EventRecorderSettingsSO.GetEventRecorderVersion();
            CheckForGameIdError();
            
            window.Show();
        }

        public static void CheckForGameIdError()
        {
            GameIdHasError = !EventRecorderSettingsSO.IsGameIdValid(Settings.eventId);
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        static void DrawSpace(int num = 1)
        {
            for (int i = 0; i < num; i++)
                EditorGUILayout.Space();
        }

        void OnGUI()
        {
            if (!Settings)
                Settings = EventRecorderSettingsSO.GetInstance();

            GUILayout.Label(string.Format("Event Recorder v{0}", EventRecorderVersion), EditorStyles.miniLabel);
            
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
            //------------------Runtime Settings-----------------------
            RuntimeOptionsToggle = EditorGUILayout.BeginFoldoutHeaderGroup(RuntimeOptionsToggle, "Runtime Options");
            if (RuntimeOptionsToggle)
            {
                GUI_DrawGameId();
                GUI_DrawGameAuthCallbackUrl();
                GUI_DrawBuildUseCaseEnum();

                if (Settings.buildUseCase != BuildUseCase.Development)
                    GUI.enabled = false;

                Settings.storeEventsPermanently =
                    EditorGUILayout.Toggle("Use Permanent Storage", Settings.storeEventsPermanently);
                Settings.useEventRecorderLog =
                    EditorGUILayout.Toggle("Use EventRecorder Log", Settings.useEventRecorderLog);
                Settings.showRuntimeLog = EditorGUILayout.Toggle("Show Runtime Log", Settings.showRuntimeLog);
                Settings.targetEndpoint = (EventRecorderEndpointSO) EditorGUILayout.ObjectField("Target Endpoint:",
                    Settings.targetEndpoint, typeof(EventRecorderEndpointSO), false);
                if (Settings.targetEndpoint == null || Settings.buildUseCase != BuildUseCase.Development)
                {
                    Settings.targetEndpoint =
                        AssetDatabase.LoadAssetAtPath<EventRecorderEndpointSO>(
                            "Assets/EventRecorder/Endpoints/Simcoach_AWS.asset");
                    if (Settings.targetEndpoint == null)
                        Debug.LogError(
                            "Could not find default Simcoach_AWS endpoint asset! Did you move it from \"Assets/EventRecorder/Endpoints/Simcoach_AWS.asset\"? Ask Will Pyle if you need assistance.");
                }

                GUI.enabled = true;

                Settings.postInterval = EditorGUILayout.FloatField("Post Interval (Seconds): ", Settings.postInterval);
                if (Settings.postInterval < 3)
                    EditorGUILayout.HelpBox("Attempting to post this constantly could have performance implications!",
                        MessageType.Warning);
                
                Settings.recordUnityApplicationEvents =
                    EditorGUILayout.Toggle("Record App Events", Settings.recordUnityApplicationEvents);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            //--------------------------------------------------------

            DrawSpace(2);
            
            //------------------Editor Settings-----------------------
            EditorOptionsToggle = EditorGUILayout.BeginFoldoutHeaderGroup(EditorOptionsToggle, "Editor Options");
            if (EditorOptionsToggle)
            {
                Settings.debug_showConsoleMessages =
                    EditorGUILayout.Toggle("Show Console Messages:", Settings.debug_showConsoleMessages);
                ShowFakeServerResponseOption();

                if (Settings.debug_fakeServerResponses)
                    Settings.debug_serverPostShouldFail =
                        EditorGUILayout.Toggle("Post Fail:", Settings.debug_serverPostShouldFail);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            //-------------------------------------------------------

            EditorGUILayout.EndScrollView();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUILayout.Button("Open Persistant Data Path"))
                Application.OpenURL(Application.persistentDataPath);

            GUILayout.FlexibleSpace();

            EditorUtility.SetDirty(Settings);
        }

        static void GUI_DrawGameId()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField("Event Id", Settings.eventId);
            GUI.enabled = true;
            if (GUILayout.Button("Change"))
            {
                EventIdValidationWindow.Init();
            }
            EditorGUILayout.EndHorizontal();
            if (GameIdHasError)
            {
                EditorGUILayout.HelpBox("Invalid Game Id", MessageType.Error);
            }
        }

        static void GUI_DrawGameAuthCallbackUrl()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField("Auth Callback URL", Settings.authCallbackUrl);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        static void ShowFakeServerResponseOption()
        {
            Settings.debug_fakeServerResponses =
                EditorGUILayout.Toggle("Fake Server Responses:", Settings.debug_fakeServerResponses);

            string helpBoxMsg = Settings.debug_fakeServerResponses
                ? "FAKE server responses based on below settings. Data is not actually sent to the backend."
                : "REAL server responses. Data sent to the backend is marked as 'debug'.";

            EditorGUILayout.HelpBox(helpBoxMsg, MessageType.Info);
        }

        static void GUI_DrawBuildUseCaseEnum()
        {
            var newUseCase = (BuildUseCase) EditorGUILayout.EnumPopup("Build Use-case", Settings.buildUseCase);
            if (newUseCase != Settings.buildUseCase)
            {
                Settings.ChangeBuildUseCase(newUseCase);
            }

            string helpBoxText = "";
            switch (Settings.buildUseCase)
            {
                case BuildUseCase.Development:
                    helpBoxText = "Use for general testing. Events tagged as \"Debug\" See optional settings below.";
                    break;
                case BuildUseCase.PlayTest:
                    helpBoxText =
                        "Events are saved as production level events. Only use for play tests! Events saved to a permanent backup file. Event Recorder runtime logs saved to disk.";
                    break;
                case BuildUseCase.AppStore:
                    helpBoxText =
                        "Events are saved as production level events. Events only saved to backlog and removed after a successful post.";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.HelpBox(helpBoxText, MessageType.Info);
        }
    }
}