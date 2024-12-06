using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Simcoach.SkillArcade
{
    public class SkillArcadeUIManager : MonoBehaviour
    {
        public static SkillArcadeUIManager instance;

        //set in inspector to identify scene to auto-show message banner on load
        public bool inMainScene;

        public float delayBeforeShowingMainButton;
        public float delayBeforeShowingBanner;
        public float delayBeforeHidingBanner;

        public UI.OverlayFadeController overlayFade;
        public UI.OverlayFadeController windowFade;

        public UI.LoadingPanel mainOverlayLoadingPanel;
        public UI.SkillArcadeButton skillArcadeButton;
        public UI.SAMessageBanner messageBanner;
        public UI.SimplePanel signInPanel;
        public UI.SimplePanel mainMenuPanel;
        public UI.LoadingPanel signInLoadingPanel;
        public UI.ErrorPanel signInErrorPanel;
        public UI.SimplePanel emailCertificateChickenbox;
        public Text emailCertificateChickenboxMessage;
        public UI.SimplePanel emailCertificateNotification;
        public Text emailCertificateNotificationMessage;
        public UI.SimplePanel locationPermissionWindow;
        public UI.SimplePanel locationServicesOffWindow;

        public SAAdPopup dynamicAdPopup;

        public UI.SABadgePopup badgePopup;
        public UI.SimplePanel signInPromptPopup;
        public UI.SimplePanel gameLockedWarningPopup;
        bool sawSignInPromptThisSession;

        public InputField emailField;
        public InputField passwordField;
        public GameObject emailSuccessObject;
        public GameObject emailFailObject;

        string createAccountWebLink = "https://profile.simcoachskillarcade.com/signup/";
        string changePasswordWebLink = "https://profile.simcoachskillarcade.com/changepassword";
        string profileWebLink = "https://profile.simcoachskillarcade.com/profile/";
        string profileQueryString = "?browser=plugin";
        string badgeQueryString = "?browser=plugin&game=simcoachgames." + GameIdentifiers.gameID;

        bool loginLoading;
        bool autoLoginActive;

        bool showingMainButtonCoroutine = false;

        public string bronzeBadgeTitle;
        public string bronzeBadgeDesc;
        public string bronzeBadgeTitle_sp;
        public string bronzeBadgeDesc_sp;
        public Texture2D bronzeBadgeSprite;
        bool shownBronzeBadge;
        
        private string welcomKey = "Welcome";
        private string helloKey = "Hello";
        private string signInKey = "SignIn?";

        WebViewController webViewController;

        bool suppressMainButton = false;

        public Text zipDebugText;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(transform.parent.parent.gameObject);
            }

            DontDestroyOnLoad(transform.parent.parent.gameObject);

            webViewController = GetComponent<WebViewController>();
        }

        // Use this for initialization
        void Start()
        {
            Simcoach.Net.WebComms.Init();
            Simcoach.Net.GenerateUUID.Init(); //Generate globally unique identifier
                        
            skillArcadeButton.SetTouchable(false);

            /*
            if (inMainScene)
            {
                string accessToken = PlayerPrefs.GetString("accessToken", "");
                string refreshToken = PlayerPrefs.GetString("refreshToken", "");
                autoLoginActive = false;

                //if the user has logged in an stored the access token, check for it and try to get profile
                if (accessToken != "" && refreshToken != "" && !Simcoach.Net.WebComms.LoggedIn())
                {
                    mainOverlayLoadingPanel.Show();
                    Simcoach.Net.WebComms.accessToken = accessToken;
                    Simcoach.Net.WebComms.refreshToken = refreshToken;
                    Simcoach.Net.WebComms.RequestRefreshToken();
                    autoLoginActive = true;
                }
                else
                {
                    //start the scene load for unlogged in state
                    StartCoroutine("InitialSceneLoad");   
                }
                
                //if (!Simcoach.Net.WebComms.LoggedIn())
                //    ShowSignInPrompt();
            }

            emailField.text = PlayerPrefs.GetString("SA_Email", "");
            BadgeController.CreateInstance();
            if (Net.WebComms.LoggedIn())
                EventManagerBase.ReadyToShowBadges();
            */

            //DynamicAdManager.instance.ShowMainMenuAds(2);

        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (EventManagerBase.GetShouldShowBadges())
            {
                CheckForBadges();
            }
            */

            zipDebugText.text = CurrentSkiller.GetZipCode();
        }

        public void NagUserToSignUp()
        {
            //only bug not logged in users
            if (!Simcoach.Net.WebComms.LoggedIn() && !sawSignInPromptThisSession)
            {
                ShowSignInPrompt();
                sawSignInPromptThisSession = true;
            }
        }
        
        public void MainButton_OnTap()
        {
            HideSignInPrompt();
            HideLockedWarningPrompt();
            
            if (Simcoach.Net.WebComms.LoggedIn())
                mainMenuPanel.Toggle();
            else
                signInPanel.Toggle();

            if (mainMenuPanel.showing || signInPanel.showing)
            {
                if (Simcoach.Net.WebComms.LoggedIn())
                    messageBanner.ShowMessage(welcomKey, CurrentSkiller.firstName + " " + CurrentSkiller.lastName, false, false);
                else
                    messageBanner.ShowMessage(helloKey, signInKey, false);

                overlayFade.Darken();
            }
            else
            {
                CloseOverlay();
            }
        }

        public void LoginButton_OnTap()
        {
            autoLoginActive = false;
            loginLoading = true;
            signInLoadingPanel.Show();
            Simcoach.Net.WebComms.BasicLogin(emailField.text, passwordField.text);
            PlayerPrefs.SetString("SA_Email", emailField.text);
        }

        /// <summary>
        /// Called by WebComm when a login request has finished.
        /// </summary>
        /// <param name="responseTimeout">Did the request timeout?</param>
        /// <param name="statusCode">Request's status code.</param>
        /// <param name="responseCode">Requests response code (part of json package)</param>
        public void HandleLoginRequestResponse(bool responseTimeout, int statusCode, int responseCode)
        {
            loginLoading = false;
            if (responseTimeout)
            {
                UnsuccessfulLogin("Timeout");
                SkillArcade.EventManagerBase.BadgeEventFailed();
            }
            else
            {
                if (statusCode == -1)
                {
                    UnsuccessfulLogin("Unspecified error");
                    SkillArcade.EventManagerBase.BadgeEventFailed();
                }
                else if (statusCode == 200)
                {
                    if (responseCode == 10500)
                        Simcoach.Net.WebComms.GetProfile();
                    else
                    {
                        UnsuccessfulLogin("Login Error: " + Simcoach.Net.APIErrorMessages.GetErrorMessage(responseCode));
                        SkillArcade.EventManagerBase.BadgeEventFailed();
                    }
                }
                else
                {
                    UnsuccessfulLogin("Login error: " + statusCode.ToString());
                    SkillArcade.EventManagerBase.BadgeEventFailed();
                }
            }

        }

        /// <summary>
        /// Called by WebComm when a GetProfile request has finished.
        /// </summary>
        /// <param name="responseTimeout">Did the request timeout?</param>
        /// <param name="statusCode">Request's status code.</param>
        /// <param name="responseCode">Requests response code (part of json package)</param>
        public void HandleGetProfileResponse(bool responseTimeout, int statusCode, int responseCode)
        {
            if (responseTimeout)
                Debug.Log("Timeout on GetProfile()");
            else
            {
                if (statusCode == 200)
                {
                    if (responseCode == 10600)
                    {
                        if (autoLoginActive) OnSuccessfulAutoLogin();
                        else OnSuccessfulLogin();
                    }
                    else
                        UnsuccessfulLogin("Get Profile Error: " + Simcoach.Net.APIErrorMessages.GetErrorMessage(responseCode));
                }
                else
                {
                    UnsuccessfulLogin("Couldn't get Profile. (Error code: " + statusCode.ToString() + ")");
                }

            }
        }

        /// <summary>
        /// Called by WebComm when a Reresh Token request has finished.
        /// </summary>
        /// <param name="responseTimeout">Did the request timeout?</param>
        /// <param name="statusCode">Request's status code.</param>
        /// <param name="responseCode">Requests response code (part of json package)</param>
        public void HandleRequestRefreshTokenResponse(bool responseTimeout, int statusCode, int responseCode)
        {
            if (responseTimeout)
            {
                Debug.Log("Timeout on RequestRefreshToken()");

                UnsuccessfulLogin("Couldn't log in. (Error code: " + statusCode.ToString() + ")");
            }
            else
            {
                if (statusCode == 200)
                {
                    if (responseCode == 10900)
                    {
                        //Nothing, WebComms will handle getting profile info
                    }
                    else
                        UnsuccessfulLogin("Error signing in! Try signing in manually.");
                }
                else
                {
                    UnsuccessfulLogin("Couldn't log in. (Error code: " + statusCode.ToString() + ")");
                }

            }
        }

        public void OnSuccessfulLogin()
        {
            SetMessageBanner_LoggedIn();
            signInLoadingPanel.Hide();
            signInPanel.Hide();
            mainMenuPanel.Show();

            PlayerPrefs.SetString("accessToken", Simcoach.Net.WebComms.accessToken);
            PlayerPrefs.SetString("refreshToken", Simcoach.Net.WebComms.refreshToken);
            
            mainOverlayLoadingPanel.Hide();

            EventManagerBase.ReadyToShowBadges();
            Net.WebComms.loggingIn = false;
            
            //SendAppOpenedEvent();
        }

        public void OnSuccessfulAutoLogin()
        {
            //SetMessageBanner_LoggedIn();

            StartCoroutine("InitialSceneLoad");

            EventManagerBase.ReadyToShowBadges();

            Net.WebComms.loggingIn = false;

            //SendAppOpenedEvent();
        }

        public void SendAppOpenedEvent()
        {
            System.Collections.Generic.List<Pair<string, string>> gameData = new System.Collections.Generic.List<Pair<string, string>>();
            gameData.Add(new Pair<string, string>("opened", "true"));
            //EventManagerBase.SendDataEvent("appOpened", gameData);
        }

        public void UnsuccessfulLogin(string errorMessage)
        {
            Debug.Log(errorMessage);

            PlayerPrefs.SetString("accessToken", "");
            PlayerPrefs.SetString("refreshToken", "");
            Simcoach.Net.WebComms.ClearLogin();
            signInLoadingPanel.Hide();
            signInErrorPanel.Show(errorMessage);

            StartCoroutine("InitialSceneLoad");

            Net.WebComms.loggingIn = false;
        }

        public void SignOutButton_OnTap()
        {
            PlayerPrefs.SetString("accessToken", "");
            Simcoach.Net.WebComms.BasicLogout();
            messageBanner.ShowMessage(helloKey, signInKey, false);
            skillArcadeButton.HideInitials();
            mainMenuPanel.Hide();
            signInPanel.Show();
                       
        }

        public void SetMessageBanner_LoggedIn()
        {
            messageBanner.ShowMessage(welcomKey, CurrentSkiller.firstName + " " + CurrentSkiller.lastName, false, false);
            skillArcadeButton.SetInitialsText(CurrentSkiller.GetInitials());
            skillArcadeButton.ShowInitials();

            //in game scenes, show the button only when the message banner comes out
            if (!inMainScene)
            {
                skillArcadeButton.PlayAppearAnimation();
            }
        }

        public void CheckForBadges()
        {
            Debug.Log("checking for badges");
            StartCoroutine(waitForBadgeLoading());
        }

        IEnumerator waitForBadgeLoading()
        {
            while (BadgeController.loadingBadges || EventManagerBase.WaitForBadgeData())
            {
                yield return null;
            }

            Badge latestBadge = BadgeController.GetLatestBadge();
            if (latestBadge != null)
                ShowBadge(latestBadge);
        }

        public void ShowBadge(Badge newBadge, bool offline = false)
        {
            overlayFade.Darken();
            badgePopup.ShowBadge(newBadge, offline);
        }

        public void BadgePopupContinueButton_OnTap()
        {
            badgePopup.Hide();

            if (BadgeController.CheckForQueuedBadges())
            {
                CheckForBadges();
                return;
            }

            //if skiller is logged in, simply hide the badge window upon player input.
            //else show the sign-in nagger
            if (!Net.WebComms.LoggedIn())
            {
                ShowSignInPrompt();
                //ShowLockedWarningPrompt();
            }
            else
            {
                if (!signInPanel.showing && !mainMenuPanel.showing)
                    overlayFade.Undarken();
            }
        }

        public void ShowSignInPrompt()
        {
            shownBronzeBadge = false;
            overlayFade.Darken();
            signInPromptPopup.Show();
        }

        public void ShowLockedWarningPrompt()
        {
            shownBronzeBadge = false;
            overlayFade.Darken();
            gameLockedWarningPopup.Show();
        }

        public void HideSignInPrompt()
        {
            overlayFade.Undarken();
            signInPromptPopup.Hide();
        }

        public void HideLockedWarningPrompt()
        {
            overlayFade.Undarken();
            gameLockedWarningPopup.Hide();
        }

        public void SignInPrompt_SignInButton_OnTap()
        {
            signInPromptPopup.Hide();
            gameLockedWarningPopup.Hide();
            signInPanel.Show();
            skillArcadeButton.PlayAppearAnimation();
            messageBanner.ShowMessage(helloKey, signInKey, false);
        }

        public void SignInPrompt_CreateAccountButton_OnTap()
        {
            //webViewController.OpenURL(createAccountWebLink, "CREATE ACCOUNT");
            signInPromptPopup.Hide();
            gameLockedWarningPopup.Hide();
            signInPanel.Show();
            skillArcadeButton.PlayAppearAnimation();
        }

        public void SignInPanel_CreateAccountButton_OnTap()
        {
            //webViewController.OpenURL(createAccountWebLink, "CREATE ACCOUNT");
        }

        public void ForgotPasswordButton_OnTap()
        {
            //webViewController.OpenURL(changePasswordWebLink, "FORGOT PASSWORD");
        }

        public void ProfileButton_OnTap()
        {
            string fullURL = profileWebLink + Net.WebComms.accessToken + profileQueryString;
            //webViewController.OpenURL(fullURL, "SKILL ARCADE PROFILE");
        }

        public void BadgeButton_OnTap()
        {
            string fullURL = profileWebLink + Net.WebComms.accessToken + badgeQueryString;
            //webViewController.OpenURL(fullURL, "BADGES");
        }

        public void ArcadeButton_OnTap()
        {
            string fullURL = "https://redirect.simcoachskillarcade.com/MpqnjOEbiO";
#if UNITY_ANDROID
            fullURL = "https://redirect.simcoachskillarcade.com/MpqnjOEbiO";
#elif UNITY_IOS
            fullURL = "https://redirect.simcoachskillarcade.com/XpgVVAhERe";
#endif
            //webViewController.OpenURL(fullURL, "SKILL ARCADE GAMES");
        }

        public void ShowEmailCertificateChickenbox()
        {
            //overlayFade.Darken();
            windowFade.Darken();
            SetEmail();
            emailCertificateChickenbox.Show();
        }

        public void SetEmail()
        {
            //emailCertificateChickenboxMessage.text = emailCertificateChickenboxMessage.text.Replace("{address}", CurrentSkiller.emailAddr);
            emailCertificateChickenboxMessage.GetComponent<SSALocalizedText>().SetKey("Email1");
        }

        public void HideEmailCertificateChickenbox()
        {
            windowFade.Undarken();
            emailCertificateChickenbox.Hide();
        }

        public void SendEmailCertificateButton_OnClick()
        {
            Net.WebComms.SendCertificateEmail(CurrentSkiller.emailAddr, CurrentSkiller.firstName, CurrentSkiller.skid);
            HideEmailCertificateChickenbox();
        }

        public void SendEmailCertificateButtonAll_OnClick()
        {
            Net.WebComms.SendCertificateEmailAll(CurrentSkiller.emailAddr, CurrentSkiller.firstName, CurrentSkiller.skid);
            HideEmailCertificateChickenbox();
        }

        public void EmailSentSuccessfully()
        {
            emailSuccessObject.SetActive(true);
            emailFailObject.SetActive(false);
            emailCertificateNotificationMessage.GetComponent<SSALocalizedText>().SetKey("Email2");
            emailCertificateNotification.Show();
            HideEmailCertificateChickenbox();
        }

        public void EmailFailed()
        {
            emailSuccessObject.SetActive(false);
            emailFailObject.SetActive(true);
            emailCertificateNotification.Show();
            HideEmailCertificateChickenbox();
        }

        public void ShowLocationPermissionWindow()
        {
            overlayFade.Darken();
            locationPermissionWindow.Show();
        }

        public void LocationPermissionAllow_OnTap()
        {
            //Net.LocationService.instance.RequestPermission();
            CloseOverlay();
        }

        public void LocationPermissionDeny_OnTap()
        {
            CloseOverlay();
        }

        public void ShowLocationServicesOffWindow()
        {
            overlayFade.Darken();
            locationServicesOffWindow.Show();
        }

        public void HideLocationServicesOffWindow()
        {
            CloseOverlay();
        }

        public void Darkener_OnTap()
        {
            if (shownBronzeBadge)
            {
                badgePopup.Hide();
                ShowSignInPrompt();
                //ShowLockedWarningPrompt();
                shownBronzeBadge = false;
            }
            else
                CloseOverlay();
        }

        public void WindowDarkener_OnTap()
        {
            HideWindows();
        }

        void CloseOverlay()
        {
            signInPromptPopup.Hide();
            gameLockedWarningPopup.Hide();
            badgePopup.Hide();
            signInPanel.Hide();
            mainMenuPanel.Hide();
            messageBanner.Hide();
            dynamicAdPopup.Hide();
            locationPermissionWindow.Hide();
            locationServicesOffWindow.Hide();
            emailCertificateChickenbox.Hide();
            emailCertificateNotification.Hide();
            //in game scenes, hide the button when the banner goes in
            if (!inMainScene)
            {
                skillArcadeButton.PlayDisappearAnimation();
            }

            DynamicAdManager.instance.CloseFullscreenAds();

            overlayFade.Undarken();
            windowFade.Undarken();
        }

        void HideWindows()
        {
            signInPromptPopup.Hide();
            gameLockedWarningPopup.Hide();
            badgePopup.Hide();
            emailCertificateChickenbox.Hide();
            emailCertificateNotification.Hide();
			DynamicAdManager.instance.CloseFullscreenAds();
            locationPermissionWindow.Hide();
            locationServicesOffWindow.Hide();
            windowFade.Undarken();
        }

        public void HideMainButton()
        {

            Debug.Log("HideMainButton()");
            if (showingMainButtonCoroutine)
                StopCoroutine("InitialSceneLoad");
            
            mainOverlayLoadingPanel.Hide();
            skillArcadeButton.PlayDisappearAnimation();
            messageBanner.Hide();

            suppressMainButton = true;
        }

        public void ShowMainButton()
        {
            if (Net.WebComms.loggingIn) return;

            StartCoroutine(showMainButton());
            suppressMainButton = false;
        }

        public IEnumerator InitialSceneLoad()
        {

            showingMainButtonCoroutine = true;
            yield return new WaitForSeconds(delayBeforeShowingMainButton);
            mainOverlayLoadingPanel.Hide();
            skillArcadeButton.PlayAppearAnimation();
            yield return new WaitForSeconds(delayBeforeShowingBanner);
            if (Simcoach.Net.WebComms.LoggedIn())
                SetMessageBanner_LoggedIn();
            else
                messageBanner.ShowMessage(helloKey, signInKey, false);
            yield return new WaitForSeconds(delayBeforeHidingBanner);
            if (!signInPanel.showing && !mainMenuPanel.showing)
                messageBanner.Hide();

            showingMainButtonCoroutine = false;

            if (suppressMainButton)
            {
                HideMainButton();
                suppressMainButton = false;
            }
        }

        public IEnumerator showMainButton()
        {
            skillArcadeButton.PlayAppearAnimation();
            yield return new WaitForSecondsRealtime(1f);
            if (Net.WebComms.LoggedIn())
            {
                skillArcadeButton.SetInitialsText(CurrentSkiller.GetInitials());
                skillArcadeButton.ShowInitials();
            }
        }

        public void ShowOfflineBronzeBadge()
        {
            ShowBadge(new Badge(bronzeBadgeTitle, bronzeBadgeDesc, bronzeBadgeTitle_sp, bronzeBadgeDesc_sp, bronzeBadgeSprite), true);
            shownBronzeBadge = true;
        }
    }
}