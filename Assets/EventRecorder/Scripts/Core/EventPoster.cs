using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif

namespace SimcoachGames.EventRecorder
{
    public class EventPoster
    {
        public static bool CurrentlyPostingAllBacklogs { get; private set; }
        static readonly HashSet<Guid> BacklogsAwaitingPost = new HashSet<Guid>();

        static EventRecorderSettingsSO RecorderSettings
        {
            get { return EventRecorderSettingsSO.GetInstance(); }
        }

        public static bool WaitingForPost
        {
            get { return BacklogsAwaitingPost.Count > 0; }
        }

        //WTP TODO: Transition all instance of Action<String, contents> to this delegate
        public delegate void WebResponse(bool success, string contents);
        
        public static IEnumerator GetRoutine(string uri, WebResponse response)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();
                bool success = webRequest.result == UnityWebRequest.Result.Success;
                response.Invoke(success, success ? webRequest.downloadHandler.text : webRequest.error);
            }
        }
        
        static IEnumerator PostRoutine(string payload, Action<bool, string> handleServerResponse)
        {
            //WTP NOTE: This sends unescaped json data
            using (UnityWebRequest webRequest = new UnityWebRequest(EventRecorderSettingsSO.GetInstance().targetEndpoint.endpoint))
            {
                webRequest.method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(payload);
                webRequest.uploadHandler = new UploadHandlerRaw(bytes);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("x-api-key", EventRecorderSettingsSO.GetInstance().targetEndpoint.apiKey);
                webRequest.SetRequestHeader("Content-Type", "application/json");

#if UNITY_2020_1_OR_NEWER
                yield return webRequest.SendWebRequest();
                bool postAttemptFailed = webRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_2_OR_NEWER
            yield return webRequest.SendWebRequest();
            var postAttemptFailed = webRequest.isHttpError || webRequest.isNetworkError;
#elif UNITY_2017_1_OR_NEWER
            yield return webRequest.Send();
            var postAttemptFailed = webRequest.isHttpError || webRequest.isNetworkError;
#else
            yield return webRequest.Send();
            var postAttemptFailed = webRequest.isError
#endif

                if (handleServerResponse != null)
                    handleServerResponse.Invoke(!postAttemptFailed, webRequest.error);
            }
        }

#if DEBUG
        static IEnumerator Debug_PostRoutine(string payload, Action<bool, string> handleServerResponse)
        {
            yield return new WaitForSeconds(0.1f);

            if (handleServerResponse != null)
                handleServerResponse.Invoke(!RecorderSettings.debug_serverPostShouldFail,
                    "FAKE ERROR CODE. YOU ARE FAKING SERVER RESPONSES!");
        }
#endif

        public static void QueueBacklogForPost(Guid userId)
        {
            BacklogsAwaitingPost.Add(userId);
        }

        static string GetFormattedBacklogPostData(Guid userId, string backlogText)
        {
            return
                "{" +
                string.Format("\"user_id\": \"{0}\",", userId.ToString()) +
                string.Format("\"payload_items\": {0}", backlogText) +
                "}";
        }
        
        //WTP TODO: Currently this is the exact same as GetFormattedBacklogPostData(). Do we want to add some kind of differentiator?
        static string GetFormattedPermalogPostData(Guid userId, string permalogText)
        {
            return
                "{" +
                string.Format("\"user_id\": \"{0}\",", userId.ToString()) +
                string.Format("\"isPermalog\": \"{0}\",", true) +
                string.Format("\"payload_items\": {0}", permalogText) +
                "}";
        }

        static IEnumerator GetPostRoutine(string payload, Action<bool, string> handleServerResponse)
        {
#if DEBUG
            return RecorderSettings.debug_fakeServerResponses
                ? Debug_PostRoutine(payload, handleServerResponse)
                : PostRoutine(payload, handleServerResponse);
#else
            return PostRoutine(payload, handleServerResponse);
#endif
        }

        static IEnumerator PostBacklogRoutine(Guid userId, Action<bool> postFinishedCallback = null)
        {
            string backlogText;
            if (EventRecorderStorage.TryGetBacklogString(userId, out backlogText))
            {
                string payload = GetFormattedBacklogPostData(userId, backlogText);
                yield return GetPostRoutine(payload, (success, errorMsg) =>
                {
                    if (success)
                    {
                        EventRecorderLog.Log(string.Format("{0} | Backlog Post Successful", userId));
                        EventRecorderStorage.ClearBacklog(userId);
                        BacklogsAwaitingPost.Remove(userId);
                    }
                    else
                    {
                        EventRecorderLog.Log(string.Format("{0} | Backlog Post Failed. Server Error - {1}", userId, errorMsg));
                    }

                    if (postFinishedCallback != null)
                        postFinishedCallback.Invoke(success);
                });
            }
            else
            {
                BacklogsAwaitingPost.Remove(userId);
            }
        }

        #region Backlog Routines

        public static IEnumerator PostUpdatedBacklogsRoutine()
        {
            EventRecorderLog.Log("Trying to post updated backlogs...");

            HashSet<Guid> postSet = new HashSet<Guid>(BacklogsAwaitingPost);
            foreach (Guid userId in postSet)
                yield return PostBacklogRoutine(userId);
        }

        public static IEnumerator PostAllBacklogsRoutine(Action<bool, string> callback = null)
        {
            CurrentlyPostingAllBacklogs = true;

            DateTime startTime = DateTime.Now;
            int fails = 0;
            int successes = 0;

            EventRecorderLog.Log("------------ Looking for non-posted backlog data...");
            List<Guid> guids = EventRecorderStorage.GetAllSavedUserIds();
            
            if (guids.Count == 0) 
                EventRecorderLog.Log("------------ No backlogs to check!");

            foreach (Guid guid in guids)
            {
                yield return PostBacklogRoutine(guid, success =>
                {
                    if (success)
                    {
                        successes++;
                    }
                    else
                    {
                        fails++;
                    }
                });

                yield return new WaitForSecondsRealtime(EventRecorderSettingsSO.BacklogPostDelay);
            }

            double totalSecondsTaken = (DateTime.Now - startTime).TotalSeconds;
            EventRecorderLog.Log(string.Format(
                "------------ Finished attempting to post backlogs after [{0}] seconds with [{1}] successes and [{2}] failures.",
                totalSecondsTaken, successes, fails));

            if (callback != null)
            {
                if (fails > 0)
                {
                    callback.Invoke(false,
                        string.Format(
                            "Failed to upload [{0}]/[{1}] data objects. Ensure you have a stable internet connection with appropriate permissions.",
                            successes + fails, fails));
                }
                else
                {
                    callback.Invoke(true, "Data objects uploaded successfully.");
                }
            }

            CurrentlyPostingAllBacklogs = false;
        }
        #endregion

        #region PermaLog Routines

        public static IEnumerator PostAllPermalogsRoutine()
        {
            DateTime startTime = DateTime.Now;
            int fails = 0;
            int successes = 0;

            EventRecorderLog.Log("------------ Looking for permalog data...");
            List<Guid> guids = EventRecorderStorage.GetAllSavedUserIds();
            if (guids.Count == 0)
            {
                EventRecorderLog.Log("------------ No permalogs to post!");
                yield return null;
            }

            foreach (Guid guid in guids)
            {
                yield return PostPermalogRoutine(guid, success =>
                {
                    if (success)
                    {
                        successes++;
                    }
                    else
                    {
                        fails++;
                    }
                });

                yield return new WaitForSecondsRealtime(EventRecorderSettingsSO.BacklogPostDelay);
            }

            double totalSecondsTaken = (DateTime.Now - startTime).TotalSeconds;
            EventRecorderLog.Log(string.Format(
                "------------ Finished attempting to post permalogs after [{0}] seconds with [{1}] successes and [{2}] failures.",
                totalSecondsTaken, successes, fails));
        }
        
        public static IEnumerator PostPermalogRoutine(Guid userId, Action<bool> postFinishedCallback = null)
        {
            string permalogText;
            if (EventRecorderStorage.TryGetPermalogString(userId, out permalogText))
            {
                string payload = GetFormattedPermalogPostData(userId, permalogText);
                yield return GetPostRoutine(payload, (success, errorMsg) =>
                {
                    EventRecorderLog.Log(success
                        ? string.Format("{0} | Permalog Post Successful", userId)
                        : string.Format("{0} | Permalog Post Failed. Server Error - {1}", userId, errorMsg));

                    if (postFinishedCallback != null)
                        postFinishedCallback.Invoke(success);
                });
            }
        }

        #endregion
    }
}