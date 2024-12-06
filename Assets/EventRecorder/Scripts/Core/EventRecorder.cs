using System;
using UnityEngine;

public static partial class EventRecorder
{
    #region Application Events

    [Serializable] public class ApplicationStartEvent : GameEventData
    {
        public string deviceType; //DeviceType: Unknown, Handheld, Console, Desktop
        public string os;
        public string deviceModel;

        public ApplicationStartEvent()
        {
            deviceType  = SystemInfo.deviceType.ToString();
            os          = SystemInfo.operatingSystem;
            deviceModel = SystemInfo.deviceModel;
        }
    }
    [Serializable] public class ApplicationQuitEvent        : GameEventData { }
    [Serializable] public class ApplicationPauseEvent       : GameEventData { }
    [Serializable] public class ApplicationResumeEvent      : GameEventData { }
    [Serializable] public class ApplicationGainedFocusEvent : GameEventData { }
    [Serializable] public class ApplicationLostFocusEvent   : GameEventData { }
    #endregion
    
    //WTP TODO: Remove Sections
    #region Sections (TO BE REMOVED)
    
    //WTP TODO: Remove Sections
    // [Serializable] public class StartSectionEvent : GameEventData
    // {
    //     public string specifier;//Anything that describes the unit of interest
    //     public Dictionary<string, object> additionalInfo;
    // }
    
    // [Serializable] public class EndSectionEvent : GameEventData
    // {
    //     public string specifier;
    //     public bool completed;
    //     public float score; // 0 - 1
    //     public float timeTaken; //Seconds
    //     public Dictionary<string, object> additionalInfo;
    //
    //     [NonSerialized] public List<GameEvent> relevantGameEvents = new List<GameEvent>();
    //     
    //     public void AddEventData(GameEvent gameEventData)
    //     {
    //         relevantGameEvents.Add(gameEventData);
    //     }
    // }
    
    // public static bool IsSectionRunning(string sectionSpecifier)
    // {
    //     return GameEventManager.SectionData.ContainsKey(sectionSpecifier);
    // }
    
    // public static List<string> GetRunningSections()
    // {
    //     return GameEventManager.SectionData.Keys.ToList();
    // }

    /// <summary>
    /// Ends all currently running sections. Called by default during Application Quit.
    /// </summary>
    /// <param name="reason">The reason we are forcing the sections to end prematurely - i.e. "Return To Main Menu</param>
    // public static void AbortAllSections(string reason)
    // {
    //     List<string> keys = GameEventManager.SectionData.Keys.ToList();
    //     foreach (string key in keys)
    //     {
    //         RecordSectionAborted(key, reason);
    //     }
    // }
    //
    // public static void RecordSectionStart(string specifier, Dictionary<string, object> additionalInfo = null)
    // {
    //     if (GameEventManager.SectionData.ContainsKey(specifier))
    //     {
    //         EventRecorderLog.LogError($"Tried to start new section [{specifier}] but a section with the same name was already active and running for [{GameEventManager.SectionData[specifier].timeTaken}] seconds.");
    //         return;
    //     }
    //     
    //     GameEventManager.RecordEvent(new StartSectionEvent()
    //     {
    //         specifier      = specifier,
    //         additionalInfo = additionalInfo
    //     });
    //     
    //     GameEventManager.SectionData.Add(specifier, new EndSectionEvent()
    //     {
    //         specifier      = specifier,
    //         additionalInfo = additionalInfo
    //     });
    // }
    //
    // public static void RecordSectionAborted(string specifier, string abortReason)
    // {
    //     RecordSectionEnd(specifier, false, 0, new Dictionary<string, object>() {{"Reason", abortReason}});
    // }
    //
    // /// <param name="score">Should be in range [0 - 1] inclusive</param>
    // public static void RecordSectionCompleted(string specifier, float score, Dictionary<string, object> additionalInfo = null)
    // {
    //     RecordSectionEnd(specifier, true, score, additionalInfo);
    // }
    //
    // static void RecordSectionEnd(string specifier, bool completed, float score, Dictionary<string, object> additionalInfo = null)
    // {
    //     if (!GameEventManager.SectionData.TryGetValue(specifier, out EndSectionEvent endSectionEvent))
    //     {
    //         EventRecorderLog.LogError($"Tried to end section [{specifier}] but no section with that name was active.");
    //         return;
    //     }
    //
    //     if (endSectionEvent.relevantGameEvents.Count > 0)
    //     {
    //         if (endSectionEvent.additionalInfo == null) 
    //             endSectionEvent.additionalInfo = new Dictionary<string, object>();
    //         
    //         endSectionEvent.additionalInfo.Add("RelevantEvents", endSectionEvent.relevantGameEvents);    
    //     }
    //     
    //     endSectionEvent.completed = completed;
    //     endSectionEvent.score = score;
    //
    //     //Throws an error if score is not of unit size
    //     if (score < 0 || score > 1)
    //     {
    //         Debug.LogError($"Error adding score data point to Section [{endSectionEvent.specifier}]. Score value of [{score}] falls outside of the valid range of 0.0f - 1.0f");
    //         return;
    //     }
    //     
    //     //Adds the StartSection and EndSection dictionaries together and throws an error if keys conflict
    //     if (additionalInfo != null)
    //     {
    //         if (endSectionEvent.additionalInfo == null)
    //         {
    //             endSectionEvent.additionalInfo = additionalInfo;
    //         }
    //         else
    //         {
    //             foreach (KeyValuePair<string, object> keyValuePair in additionalInfo)
    //             {
    //                 if (!endSectionEvent.additionalInfo.TryAdd(keyValuePair.Key, keyValuePair.Value))
    //                 {
    //                     EventRecorderLog.LogError(
    //                         $"Error adding additional data point to Section [{endSectionEvent.specifier}]. " +
    //                         $"Could not add key [{keyValuePair.Key}] with value [{keyValuePair.Value}] " +
    //                         $"because entry with same key already exists as part of the section's additional data dictionary" +
    //                         $" with value [{endSectionEvent.additionalInfo[keyValuePair.Key]}]. Either you are using the " +
    //                         $"same key for two different data points or are trying to add the same data point both in " +
    //                         $"StartSection and EndSection. If you are trying to add a data point that starts with one value" +
    //                         $" and ends with another consider using two different keys instead of trying to overwrite the data.");
    //                 }
    //             }
    //         }
    //     }
    //
    //     GameEventManager.RecordEvent(endSectionEvent);
    //     GameEventManager.SectionData.Remove(specifier);
    // }
    #endregion
}
