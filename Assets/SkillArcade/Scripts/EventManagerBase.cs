using Simcoach.SimpleJSON;
using System.Collections.Generic;
using UnityEngine;

namespace Simcoach.SkillArcade
{

    public class EventVar
    {
        public delegate string GetValueFromGame();
        public GetValueFromGame getFunction;

        public string jsonTitle { get; set; }
        public string jsonProp { get; set; }

        public EventVar(string _jsonTitle, string _jsonProp, GetValueFromGame _getFunction)
        {
            jsonTitle = _jsonTitle;
            jsonProp = _jsonProp;
            getFunction = _getFunction;
        }

    }

    public static class EventManagerBase
    {
        public static List<EventVar> eventVars = new List<EventVar>();

        static bool UIManagerShouldShowBadges = false;
        static bool waitingForBadgeData = true;

        private static bool beenInitilized = false;


        public static void Init()
        {
            eventVars.Add(new EventVar(GameIdentifiers.bronzeBadgeCompletedTag + "Change", GameIdentifiers.bronzeBadgeCompletedTag + "Completed", () => { return (PlayerPrefs.GetInt(CurrentSkiller.skid + GameIdentifiers.bronzeBadgeCompletedTag, 0) == 1).ToString().ToLower(); }));
            eventVars.Add(new EventVar(GameIdentifiers.silverBadgeCompletedTag + "Change", GameIdentifiers.silverBadgeCompletedTag + "Completed", () => { return (PlayerPrefs.GetInt(CurrentSkiller.skid + GameIdentifiers.silverBadgeCompletedTag, 0) == 1).ToString().ToLower(); }));
            eventVars.Add(new EventVar(GameIdentifiers.goldBadgeCompletedTag + "Change", GameIdentifiers.goldBadgeCompletedTag + "Completed", () => { return (PlayerPrefs.GetInt(CurrentSkiller.skid + GameIdentifiers.goldBadgeCompletedTag, 0) == 1).ToString().ToLower(); }));

            beenInitilized = true;
            waitingForBadgeData = true;
        }

        public static void GiveBronzeBadge()
        {
            PlayerPrefs.SetInt(CurrentSkiller.skid + GameIdentifiers.bronzeBadgeCompletedTag, 1);
        }

        public static void GiveSilverBadge()
        {
            PlayerPrefs.SetInt(CurrentSkiller.skid + GameIdentifiers.silverBadgeCompletedTag, 1);
        }

        public static void GiveGoldBadge()
        {
            PlayerPrefs.SetInt(CurrentSkiller.skid + GameIdentifiers.goldBadgeCompletedTag, 1);
        }


        //package the event variables into a json data package   
        public static void SendBadgeEvent()
        {
            if (!beenInitilized)
            {
                Init();
            }

            //generate json string
            string eventJsonData = "\"events\":[";

            for (int i = 0; i < eventVars.Count; i++)
            {
                string eventTitle = eventVars[i].jsonTitle;
                string eventProp = eventVars[i].jsonProp;
                string eventValue = eventVars[i].getFunction();

                eventJsonData += "{\"event\":\"" + eventTitle + "\",\"data\": {\"" + eventProp + "\":" + eventValue + "}}";

                if (i < eventVars.Count - 1)
                {
                    eventJsonData += ",";
                }
            }

            eventJsonData += "]";

            waitingForBadgeData = true;
            Net.WebComms.SendBadgeEvent(eventJsonData);
        }

        public static void SendDataEvent(string eventName, List<Pair<string, string>> data)
        {
            if (!beenInitilized)
            {
                Init();
            }

            //generate json string
            string dataJsonData = "\"events\":[{ \"event\": \"" + eventName + "\", \"data\" : {";

            for (int i = 0; i < data.Count; i++)
            {
                string title = data[i].One;
                string value = data[i].Two.ToLower();

                dataJsonData += "\"" + title + "\": " + value.ToString();

                if (i < data.Count - 1)
                {
                    dataJsonData += ",";
                }
            }

            dataJsonData += "}}]";

            Net.WebComms.SendGameData(dataJsonData);
        }

        //Parses out the json packaged returned from a SendEvent request and adds new badges to show if applicable
        public static void ParseEventReturnData(string jsonData)
        {
            Debug.Log("Badge Data:" + jsonData);
            waitingForBadgeData = false;

            if (jsonData == null || jsonData == "")
            {
                return;
            }

            JSONNode result = JSON.Parse(jsonData);
            int numDataReturned = result["data"].Count;
            if (numDataReturned == 0)
            {
                return;
            }

            string title = "";
            string desc = "";
            string title_sp = "";
            string desc_sp = "";
            string url = "";

            foreach (JSONNode badgeData in result["data"].Childs)
            {
                title = badgeData["badge"]["title"];
                desc = badgeData["badge"]["description"];
                url = badgeData["badge"]["image"];
                title_sp = badgeData["badge"]["title_sp"];
                desc_sp = badgeData["badge"]["description_sp"];
                url = badgeData["badge"]["image"];
                BadgeController.AddNewBadge(title, desc, title_sp, desc_sp, url);
            }
        }

        public static void BadgeEventFailed()
        {
            waitingForBadgeData = false;
        }

        /// <summary>
        /// Checks if UI should show badges and then sets to false
        /// </summary>
        /// <returns></returns>
        public static bool GetShouldShowBadges()
        {
            bool ret = UIManagerShouldShowBadges;
            UIManagerShouldShowBadges = false;

            return ret;
        }

        public static bool WaitForBadgeData()
        {
            return waitingForBadgeData;
        }

        public static void ReadyToShowBadges()
        {
            UIManagerShouldShowBadges = true;
        }
    }
}