using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderStorage
    {
        static EventRecorderSettingsSO RecorderSettings
        {
            get { return EventRecorderSettingsSO.GetInstance(); }
        }

        //WTP TODO: This seems unnecessary. Can we get away with just using the EventBuffers Dict?
        static readonly HashSet<Guid> BuffersUpdated = new HashSet<Guid>();
        static Dictionary<Guid, string> EventBuffers = new Dictionary<Guid, string>();

        public static bool BuffersReady
        {
            get { return BuffersUpdated.Count > 0; }
        }

        static string GetUserDirPath(Guid userId)
        {
            return Path.Combine(Application.persistentDataPath,
                string.Format("{0}{1}{2}", EventRecorderSettingsSO.UserDirPrefix,
                    EventRecorderSettingsSO.UserDirSeparator,
                    userId.ToString()));
        }

        static string GetBacklogFilePath(Guid userId)
        {
            return Path.Combine(GetUserDirPath(userId),
                string.Format("{0}{1}.json", EventRecorderSettingsSO.BacklogFilePrefix, userId.ToString()));
        }

        static string GetPermalogFilePath(Guid userId)
        {
            return Path.Combine(GetUserDirPath(userId),
                string.Format("{0}{1}.json", EventRecorderSettingsSO.PermalogFilePrefix, userId.ToString()));
        }

        static void InitUserDir(Guid userId)
        {
            Directory.CreateDirectory(GetUserDirPath(userId));

            string path = GetBacklogFilePath(userId);
            if (!File.Exists(path))
                File.Create(path).Close();

            path = GetPermalogFilePath(userId);
            if (!File.Exists(path))
                File.Create(path).Close();
        }

        public static void ResetEventBuffers()
        {
            BuffersUpdated.Clear();
            EventBuffers = new Dictionary<Guid, string>();
        }

        public static void StoreEvent(Guid userId, string gameEventJson)
        {
            InitUserDir(userId);

            gameEventJson += ",\n";

            BuffersUpdated.Add(userId);
            if (EventBuffers.ContainsKey(userId))
            {
                EventBuffers[userId] += gameEventJson;
            }
            else
            {
                EventBuffers.Add(userId, gameEventJson);
            }
        }

        public static void WriteBuffersToDisk()
        {
            EventRecorderLog.Log("Writing Buffers...");

            foreach (Guid userId in BuffersUpdated)
            {
                string saveString;
                if (!EventBuffers.TryGetValue(userId, out saveString))
                    return;

                File.AppendAllText(GetBacklogFilePath(userId), saveString);
                if (RecorderSettings.storeEventsPermanently)
                    File.AppendAllText(GetPermalogFilePath(userId), saveString);

                EventPoster.QueueBacklogForPost(userId);

                EventBuffers[userId] = string.Empty;

                EventRecorderLog.Log(RecorderSettings.storeEventsPermanently
                    ? string.Format("{0} | Event written to Backlog + Permalog", userId)
                    : string.Format("{0} | Event written to Backlog", userId));
            }

            BuffersUpdated.Clear();
        }

        public static void ClearBacklog(Guid userId)
        {
            EventRecorderLog.Log(string.Format("{0} | Backlog Cleared", userId));
            File.WriteAllText(GetBacklogFilePath(userId), string.Empty);
        }

        //This tries to parse the backlog folders
        public static List<Guid> GetAllSavedUserIds()
        {
            List<Guid> guids = new List<Guid>();
            foreach (string directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                string[] split = directory.Split(EventRecorderSettingsSO.UserDirSeparator);
                if (Path.GetFileName(split[0]) != EventRecorderSettingsSO.UserDirPrefix)
                    continue;

                Guid guid;
                if (Guid.TryParse(split[1], out guid))
                {
                    guids.Add(guid);
                }
                else
                {
                    EventRecorderLog.LogError(string.Format("Could not parse backlog folder Guid [{0}]", split[1]));
                }
            }

            return guids;
        }

        public static bool TryGetBacklogString(Guid userId, out string backlogText)
        {
            backlogText = "";
            string path = GetBacklogFilePath(userId);
            if (!File.Exists(path))
            {
                EventRecorderLog.LogError(string.Format("{0} | No backlog found!", userId));
                return false;
            }

            //Read backlog text and format it properly as a JSON array
            backlogText = File.ReadAllText(path);
            if (string.IsNullOrEmpty(backlogText))
                return false;

            backlogText = backlogText.TrimEnd();
            backlogText = backlogText.Remove(backlogText.Length - 1);
            backlogText = string.Format("[{0}]", backlogText);

            EventRecorderLog.Log(string.Format("{0} | Backlog found", userId));
            return true;
        }

        public static bool TryGetPermalogString(Guid userId, out string permalogText)
        {
            permalogText = "";
            string path = GetPermalogFilePath(userId);
            if (!File.Exists(path))
            {
                EventRecorderLog.LogError(string.Format("{0} | No Permalog found!", userId));
                return false;
            }

            //Read permalog text and format it properly as a JSON array
            permalogText = File.ReadAllText(path);
            if (string.IsNullOrEmpty(permalogText))
                return false;

            permalogText = permalogText.TrimEnd();
            permalogText = permalogText.Remove(permalogText.Length - 1);
            permalogText = string.Format("[{0}]", permalogText);

            EventRecorderLog.Log(string.Format("{0} | Permalog found", userId));
            return true;
        }

        /*
        #region Inprogress Async work
        public static async Task WriteBuffersToDiskAsync()
        {
            EventRecorderLog.Log("Writing Buffers...");
            
            foreach (Guid userId in BuffersUpdated)
            {
                string saveString = FlushEventBuffer(userId);
                if(string.IsNullOrEmpty(saveString))
                    return;
                
                await File.AppendAllTextAsync(GetBacklogFilePath(userId), saveString);
                if (RecorderSettings.storeEventsPermanently)
                    await File.AppendAllTextAsync(GetPermalogFilePath(userId), saveString);
                
                EventPoster.QueueBacklogForPost(userId);
    
                EventRecorderLog.Log(RecorderSettings.storeEventsPermanently
                    ? $"{userId} | Event written to Backlog + Permalog"
                    : $"{userId} |Event written to Backlog");    
            }
        }
        
        public static async Task<string> GetBacklogStringAsync(Guid userId)
        {
            EventRecorderLog.Log($"Trying to get backlog for {userId.ToString()}...");
            
            string backlogText = "";
            string path = GetBacklogFilePath(userId);
            if (!File.Exists(path))
            {
                EventRecorderLog.LogError($"{userId} | No backlog found!");
                return backlogText;
            }
    
            //Read backlog text and format it properly as a JSON array
            backlogText = await File.ReadAllTextAsync(path);
            backlogText = backlogText.TrimEnd();
            backlogText = backlogText.Remove(backlogText.Length - 1);
            backlogText = $"[{backlogText}]";
            
            EventRecorderLog.Log($"{userId} | Backlog found");
            return backlogText;
        }
        
         public static async void ClearBacklogAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        
    
        #endregion
        */

    }
}