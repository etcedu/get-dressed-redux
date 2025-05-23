using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/* WTP TODO:
 *  - Add new profile button should be at the bottom of the list
 *  - Route debug methods through the Event Recorder log
 */

namespace SimcoachGames.EventRecorder.Login
{
    public class SimcoachLoginUserManagement : MonoBehaviour
    {
        //Represents a signed in user and all associated profiles. Is Serializable to disk.
        [Serializable] public class SimcoachUser
        {
            public string Username { get; private set; }
            public string Organization { get; private set; }
            public List<string> profiles = new(); //WTP TODO: This could be a hashset to enforce uniqueness
            public string lastProfile = "";

            public SimcoachUser(string username, string organization = "None")
            {
                Username = username;
                Organization = organization;
            }
        }
        
        public const string DefaultProfileName = "Guardian";
        public const string AccountDeletionURL = "https://docs.google.com/forms/d/e/1FAIpQLSepFMmkolbYx-j6wWIiHKii99xYyENW6sLQlYpYGVIZoeK6DQ/viewform";
        
        static SimcoachUser SignedInUser;
        static string ActiveProfile;
        
        public static bool IsUserSignedIn { get; private set; }
        public static string GetActiveUserName() => IsUserSignedIn ? SignedInUser.Username : GameEventManager.DefaultUsername;
        public static string GetActiveOrganization() => IsUserSignedIn ? SignedInUser.Organization : GameEventManager.DefaultOrganization;
        public static string GetActiveProfileName() => IsUserSignedIn ? ActiveProfile : GameEventManager.DefaultAnonProfileName;
        
        public static event Action OnUserSignedIn;
        public static event Action OnUserSignedOut;
        public static event Action OnProfilesUpdated;

        const string OfflineUserPrefKey = "OfflineUser";
        const string OfflineUserSignInDatePrefKey = "OfflineSignInDate";
        bool offlineMode = false;
        public static string GetOfflineUserName() => PlayerPrefs.GetString(OfflineUserPrefKey, "none");
        public static bool GetIsOfflineMode() => Instance.offlineMode;
        public static bool GetOfflineModeTimeout() => (DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(OfflineUserSignInDatePrefKey, DateTime.Now.ToString()))).TotalDays > 7;

        public static SimcoachLoginUserManagement Instance { get; private set; }
        
        [field: Header("Use if app is aimed at kids")]
        [field: SerializeField] public bool UseParentalGate { get; private set; }
        [field: SerializeField] public bool UseProfileSwitchPin { get; private set; }
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            
            // UniWebViewLogger.Instance.LogLevel = UniWebViewLogger.Level.Verbose;

            //WTP TODO: Do something if not supported!
            bool oAuthSupported = IsOAuthSupported();
        }

        public static void SwitchActiveProfile(string profile)
        {
            EventRecorderLog.Log($"Requesting switch to profile [{profile}]");
            
            if (profile != DefaultProfileName)
            {
                if (!DoesProfileExist(profile))
                {
                    EventRecorderLog.LogError($"Profile [{profile}] does not exist. Switch aborted.");
                    return;
                }
            }
            
            ActiveProfile = profile;
            SignedInUser.lastProfile = ActiveProfile;
            UpdateActiveUserProfiles();
            
            EventRecorderLog.Log($"Switched to profile [{profile}] successfully");
        }

        #region Storage
        const string UserProfilesDirName = "UserProfiles";
        const string UserProfileFileType = ".json";
        const string RefreshTokenFileName = "RefreshToken";
        
        static string UserProfilesPath => Path.Combine(Application.persistentDataPath, UserProfilesDirName);
        static string RefreshTokenFilePath => Path.Combine(Application.persistentDataPath, $"{RefreshTokenFileName}.txt");
        
        static void CreateUserProfilesDir()
        {
            Directory.CreateDirectory(UserProfilesPath);
        }
        
        static string GetUserProfilePath(string username)
        {
            return Path.Combine(UserProfilesPath, $"{username}{UserProfileFileType}");
        }

        static bool LoadOrCreateUserProfileFile(string username, bool offline, string organization = "None")
        {                
            Instance.offlineMode = offline;

            EventRecorderLog.Log($"Loading or creating UserProfile file for [{username}] with Organization [{organization}]");
            
            string path = GetUserProfilePath(username);
            if (!File.Exists(path))
            {
                EventRecorderLog.Log($"UserProfile file not found for [{username}]. Creating file...");
                CreateUserProfilesFile(username, organization);
            }
            else
            {
                EventRecorderLog.Log($"UserProfile file found for [{username}]. Loading...");
            }

            SimcoachUser simcoachUser;

            if (!EventRecorderJSONHelper.TryDeserializeObject(File.ReadAllText(path), typeof(SimcoachUser), out object localSimcoachUser))
            {
                EventRecorderLog.LogError($"UserProfile file for [{username}] did not deserialize correctly!");
                return false;
            }
            else
            {
                //check if existing profile has accurate organization info
                if (!offline && (localSimcoachUser as SimcoachUser).Organization != organization)
                {
                    SimcoachUser updatedUser = new SimcoachUser((localSimcoachUser as SimcoachUser).Username, organization);
                    updatedUser.profiles = (localSimcoachUser as SimcoachUser).profiles;
                    updatedUser.lastProfile = (localSimcoachUser as SimcoachUser).lastProfile;

                    File.Delete(path);
                    File.Create(path).Close();
                    File.WriteAllText(path, EventRecorderJSONHelper.SerializeObject(updatedUser, JsonFormatting.PRETTY));

                    EventRecorderLog.Log($"Created new UserPofile file for [{username}] at [{path}] with Organization [{organization}]");

                    simcoachUser = updatedUser;
                }
                else
                    simcoachUser = localSimcoachUser as SimcoachUser;
            }

            SignedInUser = simcoachUser as SimcoachUser;
            if(SignedInUser == null)
            {
                EventRecorderLog.LogError($"UserProfile file for [{username}] did not cast to SimcoachUser correctly!");
                return false;
            }
            
            IsUserSignedIn = true;
            
            //Try set last profile
            ActiveProfile = DoesProfileExist(SignedInUser.lastProfile) 
                ? SignedInUser.lastProfile 
                : DefaultProfileName;
            
            SignedInUser.lastProfile = ActiveProfile;
            
            EventRecorderLog.Log($"Signed in as [{SignedInUser.Username}] with Organization [{SignedInUser.Organization}]");
            OnUserSignedIn?.Invoke();

            if (!Instance.offlineMode)
            {
                PlayerPrefs.SetString(OfflineUserPrefKey, SignedInUser.Username);
                PlayerPrefs.SetString(OfflineUserSignInDatePrefKey, DateTime.Now.ToString());
            }

            return true;
        }

        static void CreateUserProfilesFile(string username, string organization = "None")
        {
            string path = GetUserProfilePath(username);
            if (File.Exists(path))
            {
                EventRecorderLog.LogError($"UserProfile file for [{username}] already exists! Cannot create new one with the same username!");
                return;
            }

            CreateUserProfilesDir();
            File.Create(path).Close();
            SimcoachUser newUser = new(username, organization);
            File.WriteAllText(path, EventRecorderJSONHelper.SerializeObject(newUser, JsonFormatting.PRETTY));
            
            EventRecorderLog.Log($"Created UserProfile file for [{username}] at [{path}] with Organization [{organization}]");
        }

        static void UpdateActiveUserProfiles()
        {
            string path = GetUserProfilePath(SignedInUser.Username);
            File.WriteAllText(path, EventRecorderJSONHelper.SerializeObject(SignedInUser, JsonFormatting.PRETTY));
            
            OnProfilesUpdated?.Invoke();
            EventRecorderLog.Log($"Updated [{path}]");
        }
           
        static void DeleteUserProfilesFile(string username)
        {
            string path = GetUserProfilePath(username);
            if (!File.Exists(path))
            {
                EventRecorderLog.LogError($"UserProfile file for [{username}] doesn't exist - Nothing to delete!");
                return;
            }

            File.Delete(path);
            EventRecorderLog.Log($"Deleted UserProfile file for [{username}]");
        }

        public static void SaveRefreshToken(string token)
        {
            File.WriteAllText(RefreshTokenFilePath, token);
        }

        public static void DeleteRefreshToken()
        {
            File.Delete(RefreshTokenFilePath);
        }
        
        public static bool TryGetRefreshToken(out string token)
        {
            token = "";
            if (File.Exists(RefreshTokenFilePath))
            {
                token = File.ReadAllText(RefreshTokenFilePath);
                return !string.IsNullOrEmpty(token);
            }

            return false;
        }
        
        #endregion

        #region Sign In Flow
        [Header("Web View")]
        [SerializeField] UniWebViewAuthenticationFlowCustomize cognitoOauthFlow;
        static string AccessToken;
        public UnityEvent onSignInSuccessful;

        void Start()
        {
            //set this based on UniWebView settings which in turn is set by retrieving the eventID
            cognitoOauthFlow.mobileRedirectUri = EventRecorderSettingsSO.GetInstance().authCallbackUrl;
            TrySignInThroughRefreshToken();
        }

        //Assigned on Unity UI
        public void OpenSignInWindow()
        {
            if (UseParentalGate)
            {
                ParentalGateManager.RequestParentalGate((result) =>
                {
                    if (result)
                    {
                        EventRecorderLog.Log("Opening sign in window...");
                        cognitoOauthFlow.StartAuthenticationFlow();            
                    }
                });   
            }
            else
            {
                EventRecorderLog.Log("Opening sign in window...");
                cognitoOauthFlow.StartAuthenticationFlow();    
            }
        }

        //Assigned on Unity UI
        public void SignOutRequested()
        {
            EventRecorderLog.Log("Sign out requested...");
            cognitoOauthFlow.StartRevokeFlow();
        }

        static bool IsOAuthSupported()
        {
            EventRecorderLog.Log("Checking OAuth 2.0 code flow support...");
            if (UniWebViewAuthenticationSession.IsAuthenticationSupported)
            {
                EventRecorderLog.Log("OAuth 2.0 code flow is supported.");
                return true;
            }

            EventRecorderLog.Log("OAuth 2.0 code flow is NOT supported on current device.");
            return false;
        }

        //If we have a refresh token, attempt to authenticate and pull user data automatically
        public bool TrySignInThroughRefreshToken()
        {
            EventRecorderLog.Log("Looking for valid refresh token...");
            
            if (TryGetRefreshToken(out string refreshToken))
            {
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    EventRecorderLog.Log("Found refresh token. Trying refresh token...");
                    cognitoOauthFlow.StartRefreshTokenFlow(refreshToken);
                    return true;
                }
            }

            EventRecorderLog.Log("No refresh token found.");
            return false;
        }

        public void OnAuthenticationFinished(UniWebViewAuthenticationStandardToken token) //Assigned in UniWebViewAuthFlow inspector
        {
            AccessToken = token.AccessToken;
            
            if(!string.IsNullOrEmpty(token.RefreshToken))
                SaveRefreshToken(token.RefreshToken);
            
            EventRecorderLog.Log("Authentication Completed. Requesting user info...");
            StartCoroutine(RequestUserInfo(
                userInfoJson =>
                {
                    Debug.Log(userInfoJson);
                    JToken emailJToken = JObject.Parse(userInfoJson)["email"];
                    if (emailJToken == null)
                    {
                        EventRecorderLog.LogError("Could not extract email from user info.");
                        return;
                    }

                    string email = emailJToken.Value<string>();
                    EventRecorderLog.Log($"Email [{email}] extracted successfully.");

                    JToken orgJToken = JObject.Parse(userInfoJson)["custom:Organization"];
                    if (orgJToken == null)
                    {
                        EventRecorderLog.LogError("Could not extract organization from user info.");
                    }

                    string organization = orgJToken != null ? orgJToken.Value<string>() : "None";
                    EventRecorderLog.Log($"Ogranization [{organization}] set.");
                    

                    LoadOrCreateUserProfileFile(email, false, organization);
                    onSignInSuccessful?.Invoke(); //Let UI know if we done!
                }));
        }
        
        //WTP TODO: Can maybe combine OnAuthenticationErrored and RefreshTokenErrored
        public static void OnAuthenticationErrored(long errorCode, string errorMessage) //Assigned in UniWebViewAuthFlow inspector 
            => EventRecorderLog.LogError($"Auth Error happened: {errorCode} {errorMessage}");
        
        //WTP TODO: Can maybe combine OnAuthenticationErrored and RefreshTokenErrored
        public static void OnRefreshTokenErrored(long errorCode, string errorMessage) //Assigned in UniWebViewAuthFlow inspector
            => EventRecorderLog.LogError($"Auth Refresh Errored: {errorCode} {errorMessage}");

        static IEnumerator RequestUserInfo(Action<string> callback)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://simcoacheventrecorder.auth.us-east-1.amazoncognito.com/oauth2/userinfo");
            www.SetRequestHeader("Content-Type", "application/x-amz-json-1.1");
            www.SetRequestHeader("Authorization", "Bearer " + AccessToken);

            //Send request
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                EventRecorderLog.LogError($"GetUserInfoErrored: Connection Failure");
                yield break;
            }

            callback.Invoke(www.downloadHandler.text);
        }
        
        public static void OnSignOutCompleted(string result) //Assigned in UniWebViewAuthFlow inspector
        {
            AccessToken = "";
            DeleteRefreshToken();
            
            SignedInUser = null;
            ActiveProfile = null;
            IsUserSignedIn = false;
            
            EventRecorderLog.Log($"Signed out user successfully");
            
            OnUserSignedOut?.Invoke();
        }
        
        public static void OnSignOutErrored(string errorMessage) //Assigned in UniWebViewAuthFlow inspector
            => EventRecorderLog.LogError($"Sign Out failed with message: {errorMessage}");
        
        #endregion

        #region Methods For Active User Profile Management
        public static bool DoesProfileExist(string profile)
        {
            return SignedInUser.profiles.Contains(profile);
        }
            
        public static bool TryAddProfile(string profileToAdd, out string msg)
        {

            //Remove whitespace before/after string 
            profileToAdd = profileToAdd.Trim();
            
            EventRecorderLog.Log($"Trying to ADD profile [{profileToAdd}] to UserProfile file for [{GetActiveUserName()}]");
            
            msg = "Profile Added";
            bool failed = false;
            
            if (SignedInUser.profiles.Contains(profileToAdd) || profileToAdd == DefaultProfileName)//Already exists
            {
                msg = "Profile already created";
                failed = true;
            }
            else if (string.IsNullOrEmpty(profileToAdd) || string.IsNullOrWhiteSpace(profileToAdd))//Name is empty
            {
                msg = "Profile name cannot be empty";
                failed = true;
            }

            if (failed)
            {
                EventRecorderLog.Log($"Declining to add profile [{profileToAdd}] to UserProfile file for [{GetActiveUserName()}] - Reason: {msg}");
                return false;
            }
            
            EventRecorderLog.Log($"Added profile [{profileToAdd}] to UserProfile file for [{GetActiveUserName()}]");
            SignedInUser.profiles.Add(profileToAdd);
            UpdateActiveUserProfiles();
            return true;
        }

        public static bool TryRemoveProfile(string profileToDelete, out string msg)
        {
            
            EventRecorderLog.Log($"Trying to DELETE profile [{profileToDelete}] from UserProfile file for [{GetActiveUserName()}]");
            
            msg = "Profile Removed";
            //Fail to remove defaultProfile or profile that doesn't exist
                
            if(profileToDelete == DefaultProfileName)
            {
                msg = $"Cannot delete {DefaultProfileName} profile";
                EventRecorderLog.Log($"Cannot delete profile [{profileToDelete}] from UserProfile file for [{GetActiveUserName()}] - Reason: {msg}");
                return false;
            }
                
            if (!SignedInUser.profiles.Contains(profileToDelete))
            {
                msg = "Tried to delete profile that doesn't exist";
                EventRecorderLog.Log($"Cannot delete profile [{profileToDelete}] from UserProfile file for [{GetActiveUserName()}] - Reason: {msg}");
                return false;
            }
                
            SignedInUser.profiles.Remove(profileToDelete);

            //If we are deleting the currently selected profile switch back to the default profile
            if (ActiveProfile == profileToDelete) 
                SwitchActiveProfile(DefaultProfileName);
            
            EventRecorderLog.Log($"Deleted profile [{profileToDelete}] from UserProfile file for [{GetActiveUserName()}]");

            UpdateActiveUserProfiles();
            return true;
        }

        public static List<string> GetProfiles()
        {
            List<string> profiles = new(SignedInUser.profiles);
            profiles.Insert(0, DefaultProfileName);
            return profiles;
        }

        public static void RequestAccountDeletion()
        {
            if (Instance.UseParentalGate)
            {
                ParentalGateManager.RequestParentalGate((result) =>
                {
                    if(result)
                        OpenAccountDeletionBrowserWindow();
                });
            }
            else
            {
                OpenAccountDeletionBrowserWindow();
            }
        }

        static void OpenAccountDeletionBrowserWindow()
        {
            //WTP HACK: Maybe there is a better place to put this? The UniWebViewWindows / UniWebViewAndroid / etc files all handle these includes already
            //We basically want to use Application Open URL on windows and editor and the safe browser on android and iOS
#if UNITY_EDITOR_WIN || (!UNITY_EDITOR_OSX && !UNITY_STANDALONE_OSX && !UNITY_IOS && !UNITY_ANDROID)
            Application.OpenURL(AccountDeletionURL);
#else
            UniWebViewSafeBrowsing safeBrowser = UniWebViewSafeBrowsing.Create(AccountDeletionURL);
            safeBrowser.Show();
#endif
        }

        public void OfflineModeLogIn()
        {
            string offlineUsername = PlayerPrefs.GetString(OfflineUserPrefKey, string.Empty);
            if (string.IsNullOrEmpty(offlineUsername))
                return;

            LoadOrCreateUserProfileFile(offlineUsername, true, "offline");
            onSignInSuccessful?.Invoke(); //Let UI know if we done!
        }

        #endregion
        
        #region Editor
#if UNITY_EDITOR
        
        [ContextMenu("Test Login Page")]
        void GetLoginCompleteHtml()
        {
            string x = Resources.Load<TextAsset>("login_complete").ToString();
        }

        [Header("Editor Testing")] 
        [SerializeField] string editor_userName = "email@email.com";
        [SerializeField] string editor_profile = "profile1";

        [ContextMenu("Sign In")]
        public void Editor_SignIn()
        {
            LoadOrCreateUserProfileFile(editor_userName, false);
            onSignInSuccessful?.Invoke(); //Let UI know if we done!
        }
        
        [ContextMenu("Sign Out")]
        public void Editor_SignOutOccured()
        {
            Instance.SignOutRequested();
        }

        [ContextMenu("Create User Profiles Dir")]
        public void Editor_CreateUserProfileDir()
        {
            CreateUserProfilesDir();
        }

        [ContextMenu("Get User Profile Path")]
        public void Editor_GetUserProfilePath()
        {
            print(GetUserProfilePath(editor_userName));
        }

        [ContextMenu("Create User")]
        public void Editor_CreateUser()
        {
            CreateUserProfilesFile(editor_userName);
        }

        [ContextMenu("Delete User")]
        public void Editor_DeleteUser()
        {
            DeleteUserProfilesFile(editor_userName);
        }

        [ContextMenu("Get Active Username")]
        public void Editor_GetActiveUser()
        {
            print(GetActiveUserName());
        }

        [ContextMenu("Get Active Profile")]
        public void Editor_GetActiveProfile()
        {
            print(GetActiveProfileName());
        }

        [ContextMenu("Add Profile")]
        public void Editor_AddProfile()
        {
            TryAddProfile(editor_profile, out string msg);
            print(msg);
        }
        
        [ContextMenu("Remove Profile")]
        public void Editor_DeleteProfile()
        {
            TryRemoveProfile(editor_profile, out string msg);
            print(msg);
        }

        [ContextMenu("Print Profiles")]
        public void Editor_PrintProfiles()
        {
            string s = "Profiles:\n";
            foreach (string profile in GetProfiles()) 
                s += $"\t-{profile}\n";
            
            print(s);
        }

        [ContextMenu("Switch To Profile")]
        public void Editor_SwitchToProfile()
        {
            SwitchActiveProfile(editor_profile);
        }
#endif
        #endregion
    }
}