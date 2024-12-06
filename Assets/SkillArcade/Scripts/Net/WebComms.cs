using UnityEngine;
using System.Collections.Generic;
using Simcoach.SimpleJSON;

namespace Simcoach.Net
{
    public static class WebComms
    {
        public static bool initialized = false;
        
        private static string _key = "LQIehv0yHE5Qc5scxa2h0aUMuD3DKLuF1WMPGT6E";
        
        private static Dictionary<string, string> _headers = new Dictionary<string, string>();

        public static string _game = "simcoachgames." + SkillArcade.GameIdentifiers.gameID;  //Modify this to change the game identifier

        public static string errorMessage;
        static string loginURL = "/skillers/login";
        public static string accessToken;
        public static string refreshToken;
        static string accessTokenExpireTime;
        static string refreshTokenExpireTime;

        static TextAsset defaultProfile;
        public static string profileData;
        public static string gameDefinitions;

        private static string authHash;
        private static string profileHash;
        private static string registerHash;
        private static string eventHash;
        private static string gameDataHash;
        private static string getBadgeHash;
        private static string refreshHash;
        private static string certificateHash;
        private static List<SkillArcade.Pair<string, SkillArcade.DynamicAd>> adHashMap;
        private static List<string> adHashStringMap;

        public static int loginCode;
        public static int getProfileCode;
        public static int registerGameCode;
        public static int eventCode;

        private static bool loggedIn;
        public static bool LoggedIn() { return (accessToken != null && accessToken != "" && !string.IsNullOrEmpty(SkillArcade.CurrentSkiller.skid));  }
        
        public static void ClearLogin() { accessToken = ""; refreshToken = ""; }

        public static bool loggingIn = false;
        
        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            
            _headers.Add("content-type", "application/json");
            _headers.Add("x-api-key", _key);

            AWSWrapper.onApiCallFinished += OnAPICallFinishedCallback;
            adHashMap = new List<SkillArcade.Pair<string, SkillArcade.DynamicAd>>();
            adHashStringMap = new List<string>();
            //initialize error messages
            APIErrorMessages.Init();
        }
        
        public static void UnInit()
        {
            initialized = false;
            _headers.Clear();
            AWSWrapper.onApiCallFinished -= OnAPICallFinishedCallback;
            APIErrorMessages.UnInit();
        }

        public static void BasicLogout()
        {
            accessToken = null;
            accessTokenExpireTime = null;
            SkillArcade.CurrentSkiller.ClearSkiller();

            //GameObject.Find("UI Root").BroadcastMessage("LogOut", null);

            Simcoach.SkillArcade.DynamicAdManager.instance.LogOut();
        }

        public static void BasicLogin(string username, string password)
        {
            if (!initialized)
                Init();

            string postData = "{\"email\": \"" + username + "\","
                             + "\"password\": \"" + password + "\"}";

            Debug.Log("Login Request: " + postData);
            AWSRequestModel request = new AWSRequestModel(loginURL, "", _headers, postData);
            authHash = AWSWrapper.ApiCall(request);

            loggingIn = true;
        }


        public static void SendCertificateEmail(string email, string firstName, string skid)
        {
            string postData = "{\"email\": \"" + email + "\","
                            + "\"firstName\": \"" + firstName + "\","
                            + "\"skid\": \"" + skid + "\","
                            + "\"gameID\": \"" + "all\"}";

            Debug.Log("Email Certificate Request");
            AWSRequestModel request = new AWSRequestModel("/skillers/sendcertificateemail", "", _headers, postData);
            certificateHash = AWSWrapper.ApiCall(request);
        }

        public static void SendCertificateEmailAll(string email, string firstName, string skid)
        {
            string postData = "{\"email\": \"" + email + "\","
                            + "\"firstName\": \"" + firstName + "\","
                            + "\"skid\": \"" + skid + "\","
                            + "\"gameID\": \"" + _game + "\"}";

            Debug.Log("Email Certificate Request");
            AWSRequestModel request = new AWSRequestModel("/skillers/sendcertificateemail", "", _headers, postData);
            certificateHash = AWSWrapper.ApiCall(request);
        }

        public static void GetProfile()
        {
            if (!initialized)
                Init();

            string postData = "{\"accessToken\": \"" + accessToken + "\"}";

            Debug.Log("Get Profile Request: " + postData);
            AWSRequestModel request = new AWSRequestModel("/skillers/getprofile", "", _headers, postData);
            profileHash = AWSWrapper.ApiCall(request);
        }
        

        public static void RegisterGame()
        {
            if (!initialized)
                Init();

            string postData = "{\"accessToken\": \"" + accessToken + "\","
                            + "\"game\": \"" + _game + "\"}";

            Debug.Log("Register Request: " + postData);
            AWSRequestModel request = new AWSRequestModel("/skillers/registergame", "", _headers, postData);
            registerHash = AWSWrapper.ApiCall(request);
        }


        public static void SendBadgeEvent(string eventData)
        {
            if (!initialized)
                Init();

            string postData = "{\"accessToken\": \"" + accessToken + "\","
                            + "\"game\": \"" + _game + "\"," 
                            + eventData + "}";

            Debug.Log("Event Request: " + postData);
            AWSRequestModel request = new AWSRequestModel("/skillers/event", "", _headers, postData);
            eventHash = AWSWrapper.ApiCall(request);
        }

        public static void SendGameData(string gameData)
        {
            if (!initialized)
                Init();

            string postData = "{\"accessToken\": \"" + accessToken + "\","
                            + "\"game\": \"" + _game + "\","
                            + gameData + "}";

            Debug.Log("Event Request: " + postData);
            AWSRequestModel request = new AWSRequestModel("/skillers/event", "", _headers, postData);
            gameDataHash = AWSWrapper.ApiCall(request);
        }

        public static void RequestRefreshToken()
        {
            if (!initialized)
                Init();

            string postData = "{\"accessToken\": \"" + accessToken + "\","
                            + "\"refreshToken\": \"" + refreshToken + "\"}";

            Debug.Log("Refresh Request");
            AWSRequestModel request = new AWSRequestModel("/skillers/refreshtoken", "", _headers, postData);
            refreshHash = AWSWrapper.ApiCall(request);

            loggingIn = true;
        }


        public static void SendGetAdRequest(string AdID)
        {
            if (!initialized)
                Init();
            
            string zip = "default";
            if (LoggedIn())
                zip = SkillArcade.CurrentSkiller.GetZipCode();
            else
                zip = "";// GetZipCode.instance.zipCodeFromLocationServices;

            string postData = "{\"game\": \"" + _game + "\","
                             + "\"adid\": \"" + AdID + "\","
                             + "\"zip\": \"" + zip + "\"}";

            Debug.Log("Ad Request: " + postData);
            AWSRequestModel request = new AWSRequestModel("/skillers/getad", "", _headers, postData);
           // Debug.Log(caller.name);
            adHashStringMap.Add(AWSWrapper.ApiCall(request));
           // adHash = AWSWrapper.ApiCall(request);
        }

        /// <summary>
        /// Callback function for AWS Requestions calls trough the wrapper
        /// </summary>
        /// <param name="response">Request response (Filled by AWSWrapper.ApiCall)</param>
        public static void OnAPICallFinishedCallback(AWSResponseModel response)
        {
            int statusCode = response.GetStatusCode();
            JSONNode result = JSON.Parse(response.apiResponse);
            string responseHash = response.apiHash;
            int responseCode = 0;

            if (result != null)
                int.TryParse(result["code"], out responseCode);

            Debug.Log("OnAPICallFinishedCallback for: " + responseHash + "  SC: " + statusCode + " result: " + response.apiResponse);

            //Reponse for Login requests
            if (responseHash == authHash)
            {
                if (statusCode == 200)
                {
                    accessToken = result["data"]["accessToken"];
                    accessTokenExpireTime = result["data"]["expireDate"];
                    refreshToken = result["data"]["refreshToken"];
                    refreshTokenExpireTime = result["data"]["refreshTokenExpireDate"];
                    loginCode = responseCode;

                    Debug.Log("Login Code: " + loginCode);
                }
                else
                {
                    Debug.Log("Error for Login: " + statusCode);
                }

                SkillArcade.SkillArcadeUIManager.instance.HandleLoginRequestResponse(response.isTimeout, statusCode, loginCode);
                return;
            }

            //Response for GetProfile requests
            if (responseHash == profileHash)
            {
                if (statusCode == 200)
                {
                    getProfileCode = responseCode;
                    if (getProfileCode == 10600)
                    {
                        SkillArcade.CurrentSkiller.ParseUserData(response.apiResponse);
                        RegisterGame();

                        //GameObject.Find("UI Root").BroadcastMessage("LogIn", null);
                        Simcoach.SkillArcade.DynamicAdManager.instance.LogIn();
                    }
                    else
                    {
                        Debug.Log(APIErrorMessages.GetErrorMessage(getProfileCode));
                        SkillArcade.EventManagerBase.BadgeEventFailed();
                    }
                }
                else
                {
                    Debug.Log("Error for GetProfile: " + statusCode);
                }

                SkillArcade.SkillArcadeUIManager.instance.HandleGetProfileResponse(response.isTimeout, statusCode, getProfileCode);
                return;
            }

            ///Reponse for Register Game requests
            if (responseHash == registerHash)
            {
                if (statusCode == 200)
                {
                    registerGameCode = responseCode;
                    Debug.Log(APIErrorMessages.GetErrorMessage(registerGameCode));

                    if (registerGameCode == 10700 || registerGameCode == 10705)
                    {
                        SkillArcade.EventManagerBase.SendBadgeEvent();
                        SkillArcade.EventManagerBase.ReadyToShowBadges();
                    }
                    else
                    {
                        Debug.Log(APIErrorMessages.GetErrorMessage(registerGameCode));
                        SkillArcade.EventManagerBase.BadgeEventFailed();
                    }
                }
                else
                {
                    Debug.Log("Error for Register Game: " + statusCode);
                }
                return;
            }

            //Reponse for SendEvent requests
            if (responseHash == eventHash)
            {
                if (statusCode == 200)
                {
                    eventCode = responseCode;

                    if (eventCode == 10800)
                    {
                        SkillArcade.EventManagerBase.ParseEventReturnData(response.apiResponse);
                    }
                    else
                    {
                        SkillArcade.EventManagerBase.BadgeEventFailed();
                        Debug.Log(APIErrorMessages.GetErrorMessage(eventCode));
                    }
                }
                else
                {
                    SkillArcade.EventManagerBase.BadgeEventFailed();
                    Debug.Log("SendEvent error: " + statusCode);
                }
                return;
            }

            //Reponse for SendGameData requests
            if (responseHash == gameDataHash)
            {
                /*
                if (statusCode == 200)
                {
                    
                    eventCode = responseCode;

                    if (eventCode == 10800)
                    {
                        //Send back back up to game-specific data class
                        SkillArcade.EventManagerBase.ParseEventReturnData(response.apiResponse);
                    }
                    else
                        Debug.Log(APIErrorMessages.GetErrorMessage(eventCode));
                }
                else
                {
                    Debug.Log("SendEvent error: " + statusCode);
                }
                */
                return;
            }

            //response for RequestRefreshToken requests
            if (responseHash == refreshHash)
            {
                if (statusCode == 200)
                {
                    eventCode = responseCode;
                    if (eventCode == 10900)
                    {
                        //we know we have valid tokens, so log the user in
                        GetProfile();
                    }
                    else
                    {
                        Debug.Log(APIErrorMessages.GetErrorMessage(eventCode));
                        SkillArcade.SkillArcadeUIManager.instance.HandleRequestRefreshTokenResponse(response.isTimeout, statusCode, getProfileCode);
                    }
                }
                else
                {
                    Debug.Log("RequestRefreshToken error: " + statusCode);
                    SkillArcade.SkillArcadeUIManager.instance.HandleRequestRefreshTokenResponse(response.isTimeout, statusCode, getProfileCode);
                }

                return;
            }

            //Reponse for SendCertificateEmail requests
            if (responseHash == certificateHash)
            {
                if (statusCode == 200)
                {
                    eventCode = responseCode;

                    if (eventCode == 12200)
                    {
                        //Send back back up to game-specific data class
                        Debug.Log(response.apiResponse);
                        SkillArcade.SkillArcadeUIManager.instance.EmailSentSuccessfully();
                    }
                    else
                    {
                        Debug.Log(APIErrorMessages.GetErrorMessage(eventCode));
                        SkillArcade.SkillArcadeUIManager.instance.EmailFailed();
                    }
                }
                else
                {
                    Debug.Log("SendEvent error: " + statusCode);
                    SkillArcade.SkillArcadeUIManager.instance.EmailFailed();
                }
            }

            for (int i = 0; i < adHashStringMap.Count; i++)
            {
                //response for SkillerGetAd requests
                if (responseHash == adHashStringMap[i])
                {
                    if (statusCode == 200)
                    {
                        eventCode = responseCode;
                        if (eventCode == 12300)
                        {
                            SkillArcade.DynamicAdManager.instance.ProcessReturnData(result["data"]);
                        }
                        else
                        {
                            Debug.Log(APIErrorMessages.GetErrorMessage(eventCode));
                        }
                    }
                    else
                    {
                        Debug.Log("RequestRefreshToken error: " + statusCode);
                    }
                    adHashStringMap.RemoveAt(i);
                }
            }
        }
    }
}
