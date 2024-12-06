using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SimcoachGames.EventRecorder.Login
{
    public class SimcoachLoginUserManagementUI : MonoBehaviour
    {
        const string PinFileName = "pin.txt"; //The current pin is stored at this path
        const string DefaultPin = "15203";// The default Pin
        public string PinPath => Path.Combine(Application.persistentDataPath, PinFileName);
        
        [Header("GAME SPECIFIC SETTINGS")]
        [SerializeField] List<string> scenesLoginIsActiveIn = new();

        [Header("Root SidePanel")] 
        [SerializeField] GameObject root;//All UI including open button
        [SerializeField] GameObject loginUI;//All UI excluding open button

        [Header("Sign In Window")] 
        [SerializeField] GameObject signInWindow;
        [SerializeField] GameObject offlineModeSignInButton;

        [Header("Welcome Window")]
        [SerializeField] GameObject welcomeWindow;
        [SerializeField] TextMeshProUGUI welcomeUserText;
        [SerializeField] TextMeshProUGUI welcomeEmailText;
        
        [Header("Keypad Window")] 
        [SerializeField] GameObject keypadWindow;
        [SerializeField] TextMeshProUGUI keypadEntryText;

        [Header("User Management Window")] [SerializeField]
        GameObject userManagementWindow;

        //WTP TODO: addedUserTemplate should be it's own class ("userProfileUIs"?) so that it can handle it's own callbacks
        [SerializeField] SimcoachLoginProfileEntryUI addedSimcoachLoginProfileTemplate;

        [SerializeField] Transform addedProfilesParent;

        //WTP TODO: Update this to be a list of all created userProfileUIs
        List<SimcoachLoginProfileEntryUI> _addedProfiles = new List<SimcoachLoginProfileEntryUI>();

        [Header("Popups")] [SerializeField] GameObject popupRoot;

        [Header("Generic Popup")] 
        [SerializeField] GameObject genericPopup;

        [SerializeField] TextMeshProUGUI genericPopupHeaderText;
        [SerializeField] TextMeshProUGUI genericPopupPromptText;
        [Space] [SerializeField] Button genericPopupTrueButton;
        [SerializeField] TextMeshProUGUI genericPopupTrueText;
        [Space] [SerializeField] Button genericPopupFalseButton;
        [SerializeField] TextMeshProUGUI genericPopupFalseText;

        [Header("Add User Popup")] 
        [SerializeField] GameObject addProfilePopup;
        [SerializeField] TMP_InputField addProfileInputField;
        [SerializeField] Button addProfileButton;
        [SerializeField] GameObject addProfileInfoObject;
        [SerializeField] TextMeshProUGUI addProfileInfoText;

        [Header("First Time Launch Window")] 
        [SerializeField] GameObject invisibleFirstTimeLaunchButton;
        [SerializeField] GameObject firstTimeLaunchWindow;
        [SerializeField] GameObject arrowPointingToLoginButton;
        string _firstTimeLaunchDoNotAskAgainKey = "EventRecorder_FirstTimeLaunchDoNotAskAgain";

        [Header("More Info")] 
        [SerializeField] GameObject moreInfoWindow;
        
        bool _shown;
        string _loadedPin; //Pin loaded from file
        string _enteredPin;//Pin entered on keypad screen
        
        void Awake()
        {
            SimcoachLoginUserManagement.OnUserSignedIn    += UpdateProfileDisplay;
            SimcoachLoginUserManagement.OnProfilesUpdated += UpdateProfileDisplay;
            SimcoachLoginUserManagement.OnUserSignedOut   += ShowSignInWindow;

            _loadedPin = LoadOrCreatePin();

            //WTP NOTE: This is handled by the GameEventManager which is a parent of this GameObject
            // DontDestroyOnLoad(gameObject);
        }
        
        string LoadOrCreatePin()
        {
            //Create pin file if not found
            if (!File.Exists(PinPath))
            {
                File.WriteAllText(PinPath, DefaultPin);
                EventRecorderLog.Log($"------------ Admin Pin not found. Creating default pin.");
                return DefaultPin;
            }

            //Read pin from file
            string pin = File.ReadAllText(PinPath).Trim();
            pin = pin.Replace(" ", "");
            bool isNumeric = int.TryParse(pin, out _);
            if (isNumeric)
            {
                EventRecorderLog.Log($"------------ Admin Pin Found");    
            }
            else
            {
                EventRecorderLog.LogError($"------------ Admin Pin [{pin}] is not valid. Resetting to default pin.");
                File.WriteAllText(PinPath, DefaultPin);
                pin = DefaultPin;
            }
            
            return pin;
        }

        void OnDestroy()
        {
            SimcoachLoginUserManagement.OnUserSignedIn    -= UpdateProfileDisplay;
            SimcoachLoginUserManagement.OnProfilesUpdated -= UpdateProfileDisplay;
            SimcoachLoginUserManagement.OnUserSignedOut   -= ShowSignInWindow;
        }

        void Start()
        {
            scenesLoginIsActiveIn.Add(SceneManager.GetActiveScene().name);
            SceneManager.sceneLoaded += (scene, mode) => UpdateLoginDisplay();
            UpdateLoginDisplay();
            
            addedSimcoachLoginProfileTemplate.gameObject.SetActive(false);
            ShowSignInWindow();
            HideUIPanel();
            
            CheckForFirstTimeLaunch();
        }
        
        void UpdateLoginDisplay()
        {
            offlineModeSignInButton.SetActive(SimcoachLoginUserManagement.GetOfflineUserName() != "none" && !SimcoachLoginUserManagement.GetOfflineModeTimeout());

            //If we are currently not active but the window is in shown position - hide it before showing it.
            if(!loginUI.activeSelf)
                HideUIPanel();

            if (scenesLoginIsActiveIn.Contains(SceneManager.GetActiveScene().name))
            {
                root.SetActive(true);    
            }
            else
            {
                HideUIPanel();
                root.SetActive(false);
            }
        }

        #region GeneralBehavior

        [ContextMenu("Show")]
        public void ShowUIPanel()
        {
            loginUI.SetActive(true);
            _shown = true;
            HideFirstTimeLaunchWindow(true);
        }

        [ContextMenu("Hide")]
        public void HideUIPanel()
        {
            loginUI.SetActive(false);
            _shown = false;
        }

        public void Toggle()
        {
            if (_shown)
                HideUIPanel();
            else
                ShowUIPanel();
        }

        void ShowWindow(GameObject window)
        {
            signInWindow.SetActive (signInWindow == window);
            welcomeWindow.SetActive(welcomeWindow == window);
            keypadWindow.SetActive (keypadWindow == window);
            userManagementWindow.SetActive(userManagementWindow == window);
            // ShowUIPanel();
        }

        public void ClosePopups()
        {
            HideGenericPopup();
            HideAddUserPopup();
        }

        #endregion
        
        #region First Time Launch

        void SetFirstLaunchKey()
        {
            PlayerPrefs.SetInt(_firstTimeLaunchDoNotAskAgainKey, 1);
        }

        void CheckForFirstTimeLaunch()
        {
            //Show the First Time Launch experience until the user either presses the simcoach logo button or the "Dont ask again" button
            int keyState = PlayerPrefs.GetInt(_firstTimeLaunchDoNotAskAgainKey, 0);
            bool dontAskAgainKeyState = keyState == 1;
            invisibleFirstTimeLaunchButton.SetActive(!dontAskAgainKeyState);
        }

        public void ShowFirstTimeLaunchWindow()
        {
            firstTimeLaunchWindow.SetActive(true);
            arrowPointingToLoginButton.SetActive(true);
            invisibleFirstTimeLaunchButton.SetActive(false);
        }

        public void HideFirstTimeLaunchWindow(bool dontShowAgain)
        {
            firstTimeLaunchWindow.SetActive(false);
            arrowPointingToLoginButton.SetActive(false);
            invisibleFirstTimeLaunchButton.SetActive(false);
            
            if(dontShowAgain)
                SetFirstLaunchKey();
        }

        #endregion

        #region MoreInfo Window

        public void ShowMoreInfo()
        {
            moreInfoWindow.SetActive(true);
        }

        public void HideMoreInfo()
        {
            moreInfoWindow.SetActive(false);
        }

        #endregion

        #region Sign In Window

        void ShowSignInWindow() => ShowWindow(signInWindow);

        
        public void OnSignInButtonPressed() => SimcoachLoginUserManagement.Instance.OpenSignInWindow();

        public void OnOfflineSignInButtonPressed()
        {
            if (SimcoachLoginUserManagement.GetOfflineModeTimeout())
            {
                return;
            }

            ShowGenericPopup(
               "Offline Mode",
               $"Are you sure you want to sign in \"{SimcoachLoginUserManagement.GetOfflineUserName()}\" in offline mode?",
               "Sign In",
               "Cancel",
               (response) =>
               {
                   if (response)
                   {
                       if (SimcoachLoginUserManagement.Instance.UseParentalGate)
                       {
                           ParentalGateManager.RequestParentalGate((result) =>
                           {
                               if (result)
                               {
                                   SimcoachLoginUserManagement.Instance.OfflineModeLogIn();
                               }
                           });
                       }
                       else
                       {
                           SimcoachLoginUserManagement.Instance.OfflineModeLogIn();
                       }
                   }
               });
        }

        #endregion

        #region Welcome Window

        public void ShowWelcomeWindow()
        {
            welcomeUserText.text = SimcoachLoginUserManagement.GetActiveProfileName();
            welcomeEmailText.text = SimcoachLoginUserManagement.GetActiveUserName();
            ShowWindow(welcomeWindow);
        }

        public void SwitchProfileButtonPressed()
        {
            
            //Decide if we should show the pin pad before letting the user get to the profile management window
            if (SimcoachLoginUserManagement.Instance.UseProfileSwitchPin)
            {
                ShowKeypadWindow();    
            }
            else
            {
                ShowProfileManagementWindow();   
            }
        }

        public void OnSignOutButtonPressed()
        {
            ShowGenericPopup(
                "Sign Out",
                $"Are you sure you want to sign out \"{SimcoachLoginUserManagement.GetActiveUserName()}\"? Future data will not be associated with your account.",
                "Sign Out",
                "Cancel",
                (response) =>
                {
                    if (response)
                    {
                        if (SimcoachLoginUserManagement.Instance.UseParentalGate)
                        {
                            ParentalGateManager.RequestParentalGate((result) =>
                            {
                                if (result)
                                {
                                    SimcoachLoginUserManagement.Instance.SignOutRequested();
                                    ShowSignInWindow();   
                                }
                            });   
                        }
                        else
                        {
                            SimcoachLoginUserManagement.Instance.SignOutRequested();
                            ShowSignInWindow();   
                        }
                    }
                });
        }

        #endregion
        
        #region Keypad Window

        void ShowKeypadWindow()
        {
            keypadEntryText.text = "";
            _enteredPin = "";
            ShowWindow(keypadWindow);
        }

        public void KeypadButtonPressed(int num)
        {
            keypadEntryText.color = Color.black;
            if (_enteredPin.Length == 0)
            {
                keypadEntryText.text = "";
            }
            _enteredPin += num.ToString();
            keypadEntryText.text += "*";
        }

        public void KeypadClearButtonPressed()
        {
            keypadEntryText.text = "";
            _enteredPin = "";
        }
        
        public void KeypadBackButtonPressed()
        {
            ShowWelcomeWindow();
        }
        
        public void KeypadSubmitButtonPressed()
        {
            if (_enteredPin == _loadedPin)
            {
                ShowProfileManagementWindow();   
            }
            else
            {
                _enteredPin = "";
                keypadEntryText.color = Color.red;
                keypadEntryText.text = "INCORRECT PIN";
            }
        }

        #endregion

        #region Profile Management Window

        void ShowProfileManagementWindow()
        {
            ShowWindow(userManagementWindow);
        }

        void UpdateProfileDisplay()
        {
            
            ShowUIPanel();
            
            //WTP TODO: Slow to delete all then re-instantiate but it really doesn't matter probably
            foreach (SimcoachLoginProfileEntryUI addedProfile in _addedProfiles) 
                Destroy(addedProfile.gameObject);

            _addedProfiles = new List<SimcoachLoginProfileEntryUI>();
            
            List<string> profiles = SimcoachLoginUserManagement.GetProfiles();
            for (int i = 0; i < profiles.Count; i++)
            {
                string profileName = profiles[i];
                
                SimcoachLoginProfileEntryUI newEntry = Instantiate(
                    addedSimcoachLoginProfileTemplate,
                    Vector3.zero, Quaternion.identity,
                    addedProfilesParent);
                
                _addedProfiles.Add(newEntry);

                newEntry.Init(
                    profileName,
                    i + 1,
                    SimcoachLoginUserManagement.GetActiveProfileName() == profileName,

                    //Show confirm select popup
                    () => ShowGenericPopup(
                        "Switch",
                        $"Change to \"{profileName}\"?",
                        "Change",
                        "Cancel",
                        (response) =>
                        {
                            if (response)
                            {
                                SimcoachLoginUserManagement.SwitchActiveProfile(profileName);
                            }
                            
                            ShowProfileManagementWindow();
                        }),

                    //Show confirm delete popup
                    () => ShowGenericPopup(
                        "Delete",
                        $"Delete profile \"{profileName}\"?",
                        "Delete",
                        "Cancel",
                        (response) =>
                        {
                            //WTP TODO: Should probably handle the error case
                            if (response)
                                SimcoachLoginUserManagement.TryRemoveProfile(profileName, out _);
                        }));
            }
        }

        public void CreateProfilePressed()
        {
            ShowAddUserPopup();
        }

        public void BackButtonPressed()
        {
            ShowWelcomeWindow();
        }
        #endregion

        #region Generic Popup

        void ShowGenericPopup(string headerText, string promptText, string trueOptionText, string falseOptionText, Action<bool> response)
        {
            genericPopupHeaderText.text = headerText;
            genericPopupPromptText.text = promptText;
            
            genericPopupTrueText.text = trueOptionText;
            genericPopupTrueButton.onClick = new Button.ButtonClickedEvent();
            genericPopupTrueButton.onClick.AddListener(HideGenericPopup);
            genericPopupTrueButton.onClick.AddListener(() => response.Invoke(true));
            
            genericPopupFalseText.text = falseOptionText;
            genericPopupFalseButton.onClick = new Button.ButtonClickedEvent();
            genericPopupFalseButton.onClick.AddListener(HideGenericPopup);
            genericPopupFalseButton.onClick.AddListener(() => response.Invoke(false));
            
            popupRoot.SetActive(true);
            genericPopup.SetActive(true);
        }

        void HideGenericPopup()
        {
            genericPopup.SetActive(false);
            popupRoot.SetActive(false);
        }

        #endregion

        #region Add Profile Popup

        void ShowAddUserPopup()
        {
            addProfileInputField.text = "";
            HideAddUserInfo();
            addProfilePopup.SetActive(true);
            popupRoot.SetActive(true);
        }

        void HideAddUserPopup()
        {
            addProfilePopup.SetActive(false);
            popupRoot.SetActive(false);
        }

        void ShowAddUserInfo(string info)
        {
            addProfileInfoText.text = info;
            addProfileInfoObject.SetActive(true);
        }

        void HideAddUserInfo()
        {
            addProfileInfoObject.SetActive(false);
        }

        //Called when the input field is updated
        public void PopupAddProfileInputUpdated(string input)
        {
            addProfileButton.interactable = !string.IsNullOrEmpty(input);
        }

        public void PopupAddProfilePressed()
        {
            if (SimcoachLoginUserManagement.TryAddProfile(addProfileInputField.text, out string errorMsg))
            {
                HideAddUserPopup();
                return;
            }
            
            ShowAddUserInfo(errorMsg);    
        }

        #endregion

        public void DeleteAccountPressed()
        {
            SimcoachLoginUserManagement.RequestAccountDeletion();
        }

        #region Editor
#if UNITY_EDITOR
        
        [ContextMenu("Set Do Not Ask Again Key")]
        public void Editor_SetFirstTimeLaunch()
        {
            SetFirstLaunchKey();
            print("Set Do Not Ask Again Key");
        }
        
        [ContextMenu("Check For Do Not Ask Again Key")]
        public void Editor_CheckForFirstTimeLaunch()
        {
            bool keySet = PlayerPrefs.GetInt(_firstTimeLaunchDoNotAskAgainKey, 0) == 1;
            print($"Do Not Ask Again Key: {keySet}");
        }
        
        [ContextMenu("Delete Do Not Ask Again Key")]
        public void Editor_DeleteFirstTimeLaunch()
        {
            PlayerPrefs.DeleteAll();
            // PlayerPrefs.DeleteKey(_firstTimeLaunchDoNotAskAgainKey);
            print("Deleted Do Not Ask Again Key");
        }
#endif
        #endregion
    }
}