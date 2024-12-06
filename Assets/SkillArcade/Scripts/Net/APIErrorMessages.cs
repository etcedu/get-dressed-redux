using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Simcoach.Net
{
    public static class APIErrorMessages
    {
        static bool initialized = false;
        static Dictionary<int, string> errorCodeDict;

        public static string GetErrorMessage(int code) {
            if (errorCodeDict.ContainsKey(code))
                return errorCodeDict[code];
            else
                return "Unknown Error Code";
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = false;

            errorCodeDict = new Dictionary<int, string>();

            //SkillerLogIn messages
            errorCodeDict[10500] = "Log in Successful";
            errorCodeDict[10501] = "Missing required arguments";
            errorCodeDict[10502] = "Database error";
            errorCodeDict[10503] = "Invalid email address";
            errorCodeDict[10504] = "Invalid password";

            //SkillerGetProfile messages
            errorCodeDict[10600] = "Get profile successful";
            errorCodeDict[10601] = "Missing required arguments";
            errorCodeDict[10602] = "Database error";
            errorCodeDict[10603] = "Invalid access token";
            errorCodeDict[10604] = "Profile not found";

            //SkillerRegisterGame messages
            errorCodeDict[10700] = "Register game successful";
            errorCodeDict[10701] = "Missing required arguments";
            errorCodeDict[10702] = "Database error";
            errorCodeDict[10702] = "Invalid access token";
            errorCodeDict[10704] = "Game not found";
            errorCodeDict[10705] = "Game already registered";

            //SkillerEvent messages
            errorCodeDict[10800] = "Event send successful";
            errorCodeDict[10801] = "Missing required arguments";
            errorCodeDict[10802] = "Invalid arguments";
            errorCodeDict[10803] = "Database error";
            errorCodeDict[10804] = "Invalid access token";
            errorCodeDict[10805] = "Game not registered";
            errorCodeDict[10806] = "No game event schemas";
            errorCodeDict[10807] = "Invalid game events";

            errorCodeDict[10900] = "Token refresh successful";
            errorCodeDict[10901] = "Missing required arguments";
            errorCodeDict[10902] = "Database error";
            errorCodeDict[10903] = "Ivalid access token";
            errorCodeDict[10904] = "Invalid refresh token";
        }

        public static void UnInit()
        {
            initialized = false;
            errorCodeDict.Clear();
        }
    }
}