
//WTP TODO: This is for my own convenience. Getting a logged in user is dependent on the login faculties actually
//being available in a given project. This should be automatically added to the project once we package this behavior up
//For now though I will use this manual define.
#define SIMCOACH_LOGIN

using System;
using System.Collections;
using SimcoachGames.EventRecorder;

#if SIMCOACH_LOGIN
using SimcoachGames.EventRecorder.Login;
#endif

using UnityEngine;

/// <summary>
/// Extended for each type of event we want to track 
/// </summary>
[Serializable] public abstract class GameEventData
{
    public Guid sessionId;  //WTP NOTE: We now always generate a session ID and a device ID. Pass the SessionId here!
    public string username; //Username of the signed in user
    public string organization; //Organization of the signed in user
    public string profile;  //Selected profile of the signed in user
} 

// GameEvent object wraps around an extended GameEventData instance to provide contextual information about the event 
[Serializable] public class GameEvent
{
    public string gameId;
    public string gameVersion;
    public string eventType;
    public string timestamp;
    public bool isDebug;
    public GameEventData eventData;
};

public class GameEventManager : MonoBehaviour
{
    //Defaults for sign in system
    public const string DefaultUsername = "Anonymous";
    public const string DefaultOrganization = "None";
    public const string DefaultAnonProfileName = "None";
    
    static GameEventManager Instance;
    public static EventRecorderSettingsSO Settings;
    
    static Coroutine BacklogRoutine;
    static bool BacklogRoutineRunning;

    static Coroutine EventProcessRoutine;

    [SerializeField] EventRecorderLog logPrefab;
    static bool Initialized = false;

    void Awake()
    {
        InitInstance();
    }

    void Start()
    {
        if(Settings.recordUnityApplicationEvents)
            RecordEvent(new EventRecorder.ApplicationStartEvent());
    }

    void OnApplicationQuit()
    {
        if(Settings.recordUnityApplicationEvents)
            RecordEvent(new EventRecorder.ApplicationQuitEvent());
    }

    void OnDestroy()
    {
        if (EventRecorderStorage.BuffersReady)
            EventRecorderStorage.WriteBuffersToDisk();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            EventRecorderLog.Log("------------ Received Application Pause Message");
            
            if(Settings.recordUnityApplicationEvents)
                RecordEvent(new EventRecorder.ApplicationPauseEvent());
            
            if (EventRecorderStorage.BuffersReady)
                EventRecorderStorage.WriteBuffersToDisk();
        }
        else
        {
            if (Initialized)
            {
                EventRecorderLog.Log("------------ Received Application Resume Message");
                
                if(Settings.recordUnityApplicationEvents)
                    RecordEvent(new EventRecorder.ApplicationResumeEvent());
                
                StartRoutines();    
            }
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            if (Settings.recordUnityApplicationEvents)
                RecordEvent(new EventRecorder.ApplicationGainedFocusEvent());
        }
        else
        {
            if (Settings.recordUnityApplicationEvents)
                RecordEvent(new EventRecorder.ApplicationLostFocusEvent());
        }
    }

    void InitInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            
            Settings = EventRecorderSettingsSO.GetInstance();
            
            //WTP TODO: Remove sections
            // SectionData = new Dictionary<string, EventRecorder.EndSectionEvent>();
            
#if UNITY_EDITOR
            if (!EventRecorderSettingsSO.IsGameIdValid(Settings.eventId))
            {
                Debug.LogError("GAME ID IS NOT VALID. Stopping EventRecorder Initialization. Events will NOT be captured.");
                return;
            }
#endif
            EventRecorderLog.Log(string.Format("------------ Starting Application: [{0}] [v{1}]", Application.identifier, Application.version));
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EventRecorderId.InitId();
            
            EventRecorderLog.Log(string.Format("------------ {0}", EventRecorderSettingsSO.GetInfoString()));
            
            StartRoutines();

            if (Settings.showRuntimeLog)
                Instantiate(logPrefab, transform);
        }

        Initialized = true;
    }
    
    void StartRoutines()
    {
        if (BacklogRoutine == null)
        {
            EventRecorderLog.Log("------------ Starting BacklogPost Routine");
            BacklogRoutine = StartCoroutine(EventPoster.PostAllBacklogsRoutine());
        }
        else
        {
            EventRecorderLog.Log("------------ BacklogPostRoutine already running");
        }

        if (EventProcessRoutine == null)
        {
            EventRecorderLog.Log("------------ Starting EventProcess Routine");
            EventProcessRoutine = StartCoroutine(EventProcessCoroutine());
        }
        else
        {
            EventRecorderLog.Log("------------ EventProcess Routine already running");
        }
    }

    static DateTime LastPost;
    static IEnumerator EventProcessCoroutine()
    {
        LastPost = DateTime.Now;
        
        while (true)
        {
            if (EventRecorderStorage.BuffersReady)
                EventRecorderStorage.WriteBuffersToDisk();

            if (EventPoster.WaitingForPost && (DateTime.Now - LastPost).TotalSeconds >= Settings.postInterval)
            {
                yield return EventPoster.PostUpdatedBacklogsRoutine();
                LastPost = DateTime.Now;
            }

            yield return new WaitForSeconds(1);
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    #region Record Event Methods

    /// <summary>
    /// Record event under the currently active user
    /// </summary>
    public static void RecordEvent(GameEventData eventData)
    {

        if (!Initialized)
        {
            EventRecorderLog.LogWarning("Event Recorder is not initialized. Doing Nothing.");
            return;
        }

        eventData.sessionId = EventRecorderId.SessionId;
        
#if SIMCOACH_LOGIN
        eventData.username = SimcoachLoginUserManagement.GetActiveUserName();
        eventData.organization = SimcoachLoginUserManagement.GetActiveOrganization();
        eventData.profile = SimcoachLoginUserManagement.GetActiveProfileName();
#else
        eventData.username = DefaultUsername;
        eventData.oranization = DefaultOraganization;
        eventData.profile = DefaultAnonProfileName;
#endif
        RecordEvent(EventRecorderId.UserId, eventData);
    }

    /// <summary>
    /// Record event under the passed in user guid
    /// </summary>
    ///
    /// WTP WARNING: This method does not associate sign in data. It expects the signed in user data to be passed in eventData already!
    public static void RecordEvent(Guid userId, GameEventData eventData)
    {
        //Classes are saved as ContainingClass+ContainingClass+ContainingClass so we need to split them and use the last one
        string[] classNames = eventData.GetType().ToString().Split('+');
        string eventType = classNames[classNames.Length - 1];

        string eventId = Settings.eventId;
        if (!EventRecorderSettingsSO.IsGameIdValid(eventId))
        {
            EventRecorderLog.LogError("Game ID is invalid or not set. Event discarded. Set an Game ID through the Event Recorder Settings Window.");
            return;
        }
        
        GameEvent gameEvent = new GameEvent()
        {
            gameId       = eventId,
            gameVersion  = Application.version,
            eventType    = eventType,
            timestamp    = DateTime.UtcNow.ToString(EventRecorderSettingsSO.TimeStampFormat),
#if DEBUG
            isDebug      = true,
#else   
            isDebug      = false,
#endif
            eventData    = eventData
        };

        string gameEventJson = EventRecorderJSONHelper.SerializeObject(gameEvent, JsonFormatting.PRETTY);
        
        EventRecorderLog.Log(string.Format("[{0}] Event Occurred", gameEvent.eventType), string.Format("[{0}] Event Occurred: {1}", gameEvent.eventType, gameEventJson));
        EventRecorderStorage.StoreEvent(userId, gameEventJson);
    }

    //WTP NOTE: This is mainly just for Relational Agent
    /// <summary>
    /// All backlogs will be posted when the app starts but, if you want to upload them all again for whatever reason you can call this method.
    /// </summary>
    /// <param name="callback">This callback will return a boolean indicating if the posting was successful. In the case of a failure it will provide a string with the reason.</param>
    public static void ForcePostBacklogs(Action<bool, string> callback)
    {
        if (EventPoster.CurrentlyPostingAllBacklogs)
        {
            callback.Invoke(false, "An automatic system is already posting data to the backend...");
            return;
        }
            
        Instance.StartCoroutine(EventPoster.PostAllBacklogsRoutine(callback));
    }

    public static void ForcePostBacklogsSafe()
    {
        LastPost = new DateTime(0);
    }
    
    //WTP NOTE: Not called by anything other than dev tooling.
    public static void PostUserPermalog(Guid userId)
    {
        Instance.StartCoroutine(EventPoster.PostPermalogRoutine(userId));
    }
    
    //WTP NOTE: Not called by anything other than dev tooling
    public static void PostAllPermalogs()
    {
        Instance.StartCoroutine(EventPoster.PostAllPermalogsRoutine());
    }
    #endregion
}
