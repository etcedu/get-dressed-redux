#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

using System.Linq;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
   public enum BuildUseCase
   {
                   // BuildType  |  EventsTagged  |  ER Log On  |  Permalog On  |  RuntimeLog  
      Development, //   Dev              Dev         Optional       Optional        Optional
      PlayTest,    //   Prod             Prod           ON             ON              ON
      AppStore     //   Prod             Prod           OFF            OFF             OFF
   }

   public class EventRecorderSettingsSO : ScriptableObject
   {
      //--Build Check Settings--
      public const bool BypassBuildChecks = false; //For Garrett ONLY!

      //--General Settings--
      public const string SettingsFilename = "EventRecorderSettings"; //Default name of the auto created instance of this SO
      public const string TimeStampFormat = "yyyy-MM-ddTHH:mm:ss.fff";
      public const float BacklogPostDelay = 1; //Delay between backlog post attempts during ER initialization
      public const string ApprovedGameIdsFile = "ApprovedGameIds";

      //--ER Log settings--
      public const string LogDirPath = "EventRecorderLogs";
      public const string LogFilePrefix = "Log_";
      public const int RuntimeLogMaxMessages = 100;

      //--User / Session file structure settings--
      public const string UserDirPrefix = "User";
      public const string UserDirSeparator = "_";
      public const string BacklogFilePrefix = "Backlog_";
      public const string PermalogFilePrefix = "Permalog_";

      //--Runtime ER log settings--
      public const int MessagePreviewLength = 60;
      public const string LogColor = "#33A851";
      public const string WarningColor = "#FFD300";
      public const string ErrorColor = "#FF3838";

      static EventRecorderSettingsSO Instance;

      public string eventId;
      public BuildUseCase buildUseCase;
      public EventRecorderEndpointSO targetEndpoint;
      public float postInterval = 60;
      public bool recordUnityApplicationEvents = true;
      public bool storeEventsPermanently;
      public bool useEventRecorderLog;
      public bool showRuntimeLog;


      static string EventRecorderVersion;

#if DEBUG
      public bool debug_showConsoleMessages;
      public bool debug_fakeServerResponses;
      public bool debug_serverPostShouldFail = false;
#endif

      public static EventRecorderSettingsSO GetInstance()
      {
         if (Instance != null)
            return Instance;

         Instance = Resources.Load<EventRecorderSettingsSO>(string.Format("EventTracker/{0}", SettingsFilename));

#if UNITY_EDITOR
         if (!Instance)
         {
            if (!Directory.Exists("Assets/Resources"))
               AssetDatabase.CreateFolder("Assets", "Resources");

            if (!Directory.Exists("Assets/Resources/EventTracker"))
               AssetDatabase.CreateFolder("Assets/Resources", "EventTracker");

            AssetDatabase.CreateAsset(
               CreateInstance<EventRecorderSettingsSO>(),
               string.Format("Assets/Resources/EventTracker/{0}.asset", SettingsFilename));

            Instance = Resources.Load<EventRecorderSettingsSO>(string.Format("EventTracker/{0}", SettingsFilename));
         }
#endif

         if (!Instance)
         {
            Debug.LogError("Could not find or create Event Recorder Settings File");
         }

         return Instance;
      }

      public static string GetEventRecorderVersion()
      {
         if (string.IsNullOrEmpty(EventRecorderVersion))
         {
            TextAsset textAsset = Resources.Load<TextAsset>("EventRecorderVersion");
            if (textAsset == null)
            {
               EventRecorderVersion = "vx.x.x";
               EventRecorderLog.LogError("Could not read Event Recorder Version From File!");
            }
            else
            {
               EventRecorderVersion = textAsset.text;
            }
         }

         return EventRecorderVersion;
      }

      public static string GetInfoString()
      {
         EventRecorderSettingsSO settings = GetInstance();
         return
            "Event Recorder Settings: " +
            string.Format("[{0}] ", GetEventRecorderVersion()) +
            string.Format("[Event Id: {0}] ",  settings.eventId) +
            string.Format("[UserId: {0}] ",    EventRecorderId.UserId) +
            string.Format("[SessionId: {0}] ", EventRecorderId.SessionId) +
            string.Format("[Use Case: {0}] ",  settings.buildUseCase) +
            string.Format("[Endpoint: {0}] ",  settings.targetEndpoint.endpoint) +
#if DEBUG
            "[Build Type: DEBUG] " +
            string.Format("[Fake Server Responses: {0}] ", settings.debug_fakeServerResponses) +
#else
         "[Build Type: PRODUCTION] | " +
#endif

            string.Format("[Post Interval: {0}] ", settings.postInterval.ToString()) +
            string.Format("[Use Permalog: {0}]", settings.storeEventsPermanently);
      }

      public static string GetInfoStringWithNewlines()
      {
         EventRecorderSettingsSO settings = GetInstance();
         return
            "Event Recorder Settings: \n" +
            string.Format("- [{0}] \n", GetEventRecorderVersion()) +
            string.Format("- [Event Id: {0}] \n",  settings.eventId) +
            string.Format("- [UserId: {0}] \n",    EventRecorderId.UserId) +
            string.Format("- [SessionId: {0}] \n", EventRecorderId.SessionId) +
            string.Format("- [Use Case: {0}] \n",  settings.buildUseCase) +
            string.Format("- [Endpoint: {0}] \n",  settings.targetEndpoint.endpoint) +
#if DEBUG
            "- [Build Type: DEBUG] \n" +
            string.Format("- [Fake Server Responses: {0}] \n", settings.debug_fakeServerResponses) +
#else
         "- [Build Type: PRODUCTION] | \n" +
#endif

            string.Format("- [Post Interval: {0}] \n", settings.postInterval.ToString()) +
            string.Format("- [Use Permalog: {0}]", settings.storeEventsPermanently);
      }

      public static bool IsGameIdValid(string gameId)
      {
         if (string.IsNullOrEmpty(gameId))
            return false;

         //TODO: Should we pull the id from the google spreadsheet? 
         
         return true;
      }

#if UNITY_EDITOR
      public void ChangeBuildUseCase(BuildUseCase newUseCase)
      {
         switch (newUseCase)
         {
            case BuildUseCase.PlayTest:
               useEventRecorderLog = true;
               storeEventsPermanently = true;
               showRuntimeLog = true;
               break;
            case BuildUseCase.AppStore:
               useEventRecorderLog = false;
               storeEventsPermanently = false;
               showRuntimeLog = false;
               break;
         }

         buildUseCase = newUseCase;
         SaveData();
      }

      public void ChangeEventId(string newId)
      {
         eventId = newId;
         SaveData();
      }

      void SaveData()
      {
         EditorUtility.SetDirty(this);
         AssetDatabase.SaveAssetIfDirty(this);
      }
#endif
   }
}