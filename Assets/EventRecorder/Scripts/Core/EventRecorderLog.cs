using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderLog : MonoBehaviour
    {
        static EventRecorderLog Instance;
        static bool Initialized;

        static FileStream FileStream;
        static StreamWriter LogWriter;

        static string CurrentTime
        {
            get { return DateTime.UtcNow.ToString(EventRecorderSettingsSO.TimeStampFormat); }
        }

        [Header("Message Log")] [SerializeField]
        GameObject runtimeLog;

        [SerializeField] EventRecorderLogMsg eventRecorderLogMsgTemplate;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] Transform msgParent;

        [Header("Message Inspector")] [SerializeField]
        GameObject eventInspector;

        [SerializeField] EventRecorderLogMsg previewMsgContainer;
        [SerializeField] Text messageInspectorFullTextUI;

        Queue<EventRecorderLogMsg> _msgInstanceQueue;

        //WTP HACK: Our runtime log is not enabled during our the EventRecorder initialization step. So I threw this class
        //together to hold the data in the MissedEventHackList until we can display them
        class MissedMessageHackStructure
        {
            public string timestamp;
            public LogType logType;
            public string message;

            public MissedMessageHackStructure(string timestamp, LogType logType, string message)
            {
                this.timestamp = timestamp;
                this.logType = logType;
                this.message = message;
            }
        }

        static List<MissedMessageHackStructure> MissedEventHackList = new List<MissedMessageHackStructure>();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                InitMsgInstancePool();

                //WTP HACK: Add missed events to the display
                foreach (MissedMessageHackStructure missedEvent in MissedEventHackList)
                    NewDisplayMessage(missedEvent.timestamp, missedEvent.logType, missedEvent.message);
            }
        }

        void OnDestroy()
        {
            if (LogWriter != null)
                LogWriter.Dispose();

            if (FileStream != null)
                FileStream.Dispose();
        }

        static void InitLogFile()
        {
            string dirPath = Path.Combine(Application.persistentDataPath, EventRecorderSettingsSO.LogDirPath);
            Directory.CreateDirectory(dirPath);
            FileStream = File.Create(Path.Combine(dirPath,
                string.Format("{0}{1}.txt", EventRecorderSettingsSO.LogFilePrefix,
                    DateTime.Now.ToString("yyyy-M-dd--HH-mm-ss"))));
            LogWriter = new StreamWriter(FileStream) {AutoFlush = true};
            Initialized = true;
        }

        #region Public Log Methods

        public static void Log(string msg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Log, msg);
            AddToScreenDisplay(timestamp, LogType.Log, msg); //WTP TODO: Strip out of production build?
        }

        public static void Log(string fileMsg, string screenMsg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Log, fileMsg);
            AddToScreenDisplay(timestamp, LogType.Log, screenMsg); //WTP TODO: Strip out of production build?
        }

        public static void LogWarning(string msg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Warning, msg);
            AddToScreenDisplay(timestamp, LogType.Warning, msg); //WTP TODO: Strip out of production build?
        }

        public static void LogWarning(string fileMsg, string screenMsg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Warning, fileMsg);
            AddToScreenDisplay(timestamp, LogType.Warning, screenMsg); //WTP TODO: Strip out of production build?
        }

        public static void LogError(string msg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Error, msg);
            AddToScreenDisplay(timestamp, LogType.Error, msg); //WTP TODO: Strip out of production build?
        }

        public static void LogError(string fileMsg, string screenMsg)
        {
            string timestamp = CurrentTime;
            WriteToLogFile(timestamp, LogType.Error, fileMsg);
            AddToScreenDisplay(timestamp, LogType.Error, screenMsg); //WTP TODO: Strip out of production build?
        }

        #endregion

        static void WriteToLogFile(string timestamp, LogType type, string msg)
        {
            if (!EventRecorderSettingsSO.GetInstance().useEventRecorderLog)
                return;
            
            if (!Initialized)
                InitLogFile();

            LogWriter.WriteLine(string.Format("{0} | [{1}] | {2}", timestamp, type.ToString().ToUpper(), msg));
        }

        static void AddToScreenDisplay(string timestamp, LogType type, string msg)
        {

            if (EventRecorderSettingsSO.GetInstance().showRuntimeLog)
            {
                if (Instance != null)
                {
                    Instance.NewDisplayMessage(timestamp, type, msg);
                }
                else
                {
                    MissedEventHackList.Add(new MissedMessageHackStructure(timestamp, type, msg));
                }
            }

#if DEBUG
            if (EventRecorderSettingsSO.GetInstance().debug_showConsoleMessages)
            {
                switch (type)
                {
                    case LogType.Log:
                        Debug.Log(string.Format("<color=cyan>[Event Recorder]</color> {0}", msg));
                        break;
                    case LogType.Warning:
                        Debug.LogWarning(string.Format("<color=cyan>[Event Recorder]</color> {0}", msg));
                        break;
                    case LogType.Error:
                        Debug.LogError(string.Format("<color=cyan>[Event Recorder]</color> {0}", msg));
                        break;
                }
            }
#endif
        }

        public static void ToggleRuntimeLog()
        {
            CloseEventInspector();
            Instance.runtimeLog.SetActive(!Instance.runtimeLog.activeInHierarchy);
        }

        public static void ShowEventInspector(EventRecorderLogMsg eventRecorderLogMsg)
        {
            Instance.previewMsgContainer.UpdateData(eventRecorderLogMsg);
            Instance.messageInspectorFullTextUI.text = eventRecorderLogMsg.Msg;
            Instance.eventInspector.SetActive(true);
        }

        public static void CloseEventInspector()
        {
            Instance.eventInspector.SetActive(false);
        }

        void InitMsgInstancePool()
        {
            _msgInstanceQueue = new Queue<EventRecorderLogMsg>(EventRecorderSettingsSO.RuntimeLogMaxMessages);
            for (int i = 0; i < EventRecorderSettingsSO.RuntimeLogMaxMessages; i++)
            {
                EventRecorderLogMsg msgInstance = Instantiate(eventRecorderLogMsgTemplate, msgParent);
                msgInstance.gameObject.SetActive(false);
                _msgInstanceQueue.Enqueue(msgInstance);
            }
        }

        IEnumerator InitMsgInstancePoolRoutine()
        {
            _msgInstanceQueue = new Queue<EventRecorderLogMsg>(EventRecorderSettingsSO.RuntimeLogMaxMessages);
            for (int i = 0; i < EventRecorderSettingsSO.RuntimeLogMaxMessages; i++)
            {
                EventRecorderLogMsg msgInstance = Instantiate(eventRecorderLogMsgTemplate, msgParent);
                msgInstance.gameObject.SetActive(false);
                _msgInstanceQueue.Enqueue(msgInstance);
                yield return new WaitForEndOfFrame();
            }
        }

        void NewDisplayMessage(string timestamp, LogType type, string msg)
        {
            EventRecorderLogMsg msgInstance = _msgInstanceQueue.Dequeue();
            msgInstance.UpdateData(timestamp, type, msg);
            msgInstance.transform.SetAsLastSibling();
            _msgInstanceQueue.Enqueue(msgInstance);
        }

        public static void OpenPersistentDataPath()
        {
            Application.OpenURL(Application.persistentDataPath);
        }
    }
}