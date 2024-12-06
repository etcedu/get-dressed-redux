using UnityEditor;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{

    public class RequestObject : MonoBehaviour {}
    
    public class EventIdValidationWindow : EditorWindow
    {
        const string IdTsvUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSt5SCr3kH7UDsp4UHOHJIJcIQnE9p592jHlFtYnC0W-EkrsWHkFf2KfU-2-JtVjyluQNYRrvz0V0MR/pub?gid=1738965917&single=true&output=tsv";
        const float WindowSize = 512;
        const string InfoText =
            "EventIds are retrieved from a google spreadsheet so make sure you are connected to the internet! " +
            "The key to retrieve your EventId is your application identifier - so make sure it is set correctly! " +
            "If you are having trouble please contact your project's producer or a senior developer.";
        
        static EventIdValidationWindow GameIdValidationWindow;
        static EventRecorderSettingsSO Settings;
        
        static RequestObject RequestObject;
        static Vector2 ScrollPos;

        static bool WaitingForServerResponse = false;
        static bool IdRetrievalError;
        static string DisplayMessage;

        public static void Init()
        {
            GameIdValidationWindow = CreateInstance<EventIdValidationWindow>();
            GameIdValidationWindow.titleContent = new GUIContent("Game Id Retriever");

            GameIdValidationWindow.ShowAuxWindow();

            float width = (Screen.currentResolution.width / 2) - (WindowSize / 2);
            float height = (Screen.currentResolution.height / 2) - (WindowSize / 2);
            GameIdValidationWindow.position = new Rect(width, height, WindowSize, WindowSize);

            IdRetrievalError = false;
            DisplayMessage = null;
        }

        void OnGUI()
        {
            if(Settings == null)
                Settings = EventRecorderSettingsSO.GetInstance();
            
            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle.richText = true;

            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
            
            GUILayout.Label("<b>Id Retriever</b>", centeredStyle);
            GUILayout.Space(20);
            GUILayout.Label(string.Format("App Id: {0}", Application.identifier), centeredStyle);

            string gameId = string.IsNullOrEmpty(Settings.eventId) ? "<color=red> NOT SET </color>" : string.Format("<color=yellow>{0}</color>", Settings.eventId);
            GUILayout.Label(string.Format("Event Id: <color=yellow>{0}</color>", gameId), centeredStyle);
            
            GUILayout.Space(20);
            
            EditorGUILayout.HelpBox(InfoText, MessageType.Info);
            
            GUI.enabled = !WaitingForServerResponse;
            if (GUILayout.Button("Retrieve")) 
                GetEventIds();
            GUI.enabled = true;
            
            if(!string.IsNullOrEmpty(DisplayMessage))
                EditorGUILayout.HelpBox(DisplayMessage, IdRetrievalError ? MessageType.Error : MessageType.None);
            
            EditorGUILayout.EndScrollView();
        }
        
        void GetEventIds()
        {
            WaitingForServerResponse = true;
            IdRetrievalError = false;
            DisplayMessage = "Retrieving Ids...";
            
            RequestObject = new GameObject("IdRequestObject").AddComponent<RequestObject>();
            RequestObject.StartCoroutine(EventPoster.GetRoutine(IdTsvUrl, ProcessGetResponse));
        }

        void ProcessGetResponse(bool success, string contents)
        {
            DestroyImmediate(RequestObject.gameObject);

            WaitingForServerResponse = false;
            IdRetrievalError = false;

            if (!success)
            {
                IdRetrievalError = true;
                DisplayMessage = string.Format("Web request failed with error: {0}", contents);
                Debug.LogError(DisplayMessage);
                return;
            }

            TsvParser.ParsedTable table = TsvParser.ParseTsv(contents);
            
            int foundRow;
            if (!table.TryFindDataInRow(Application.identifier, 0, out foundRow))
            {
                IdRetrievalError = true;
                DisplayMessage = string.Format("App Id [{0}] not found. Contact your project's producer or a senior developer", Application.identifier);
                Debug.LogError(DisplayMessage);
                return;
            }

            string eventId;
            if (!table.TryGetCell(foundRow, 1, out eventId) || string.IsNullOrEmpty(eventId))
            {
                IdRetrievalError = true;
                DisplayMessage = string.Format(
                    "App Id [{0}] has no corresponding Event Id. Contact your project's producer or a senior developer.",
                    Application.identifier);
                
                Debug.LogError(DisplayMessage);
                return;
            }
            
            Settings.ChangeEventId(eventId);
            DisplayMessage = $"{nameof(Event)} Id successfully set to [{eventId}]";
            Debug.Log(DisplayMessage);

            UniWebViewSettingsProvider.SetAuthCallbackURLs(eventId);
            if (FindObjectOfType<UniWebViewAuthenticationFlowCustomize>() != null)
                FindObjectOfType<UniWebViewAuthenticationFlowCustomize>().mobileRedirectUri = "authhub://" + eventId;

            Repaint();
        }
    }
}