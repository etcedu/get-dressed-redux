using System;
using System.IO;
using UnityEngine;

namespace SimcoachGames.EventRecorder
{
    public class EventRecorderId
    {
        const string UserIDFileName = "Users";
        static string UserIDFilePath => Path.Combine(Application.persistentDataPath, $"{UserIDFileName}.txt");

        public static bool IdsInitialized { get; private set; }
        public static Guid UserId { get; private set; }
        public static Guid SessionId { get; private set; }
        
        public static bool IsFirstLaunch { get; private set; } 

        public static void InitId()
        {
            EventRecorderLog.Log("------------ Initializing Ids");
            UserId = LoadUserIdFromFile();
            SessionId = GenerateNewSessionId();
        }
        
        public static Guid GenerateNewSessionId()
        {
            SessionId = Guid.NewGuid();
            EventRecorderLog.Log(string.Format("------------ Generating new session Id: {0}", SessionId));
            return SessionId;
        }

        //WTP TODO: We could encrypt and hide the file if we wanted
        static Guid GenerateNewUserId()
        {
            Guid newId = Guid.NewGuid();
            File.WriteAllText(UserIDFilePath, newId.ToString());
            return newId;
        }

        static Guid LoadUserIdFromFile()
        {
            EventRecorderLog.Log("------------ Looking for User Id...");
            IsFirstLaunch = false; //Set here to avoid issues with static lifetimes in editor
            
            Guid id;
            if (!File.Exists(UserIDFilePath))
            {
                IsFirstLaunch = true;
                id = GenerateNewUserId();
                EventRecorderLog.Log(string.Format("------------ User Id not found. Generated new User Id: {0}", id));
            }
            else
            {
                //Get Saved Id - Generate new one if the saved Id gets corrupted
                string txt = File.ReadAllText(UserIDFilePath);
                if (!Guid.TryParse(txt, out id))
                {
                    IsFirstLaunch = true;
                    id = GenerateNewUserId();
                    EventRecorderLog.LogError(string.Format("User Id [{0}] was invalid. Generated new User Id[{1}]", txt, id));
                }

                EventRecorderLog.Log(string.Format("------------ User Id found: {0}", id));
            }

            return id;
        }
    }
}