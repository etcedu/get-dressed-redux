using SimcoachGames.EventRecorder.Login;
using TMPro;
using UnityEngine;

public class SimcoachLicenseManagementUI : MonoBehaviour
{
    [SerializeField] GameObject mainMenuButtonsBlocker;

    [SerializeField] GameObject errorBlocker;
    [SerializeField] public TMP_Text blockerText;
    [SerializeField] TMP_Text errorBlockerText;
    [TextArea][SerializeField] string error_noOrgID;
    [TextArea][SerializeField] string error_inactiveOrgID;
    [TextArea][SerializeField] string error_invalidGameID;
    [TextArea][SerializeField] string error_connectivityError;


    const string offlineLicensePrefKey = "LicenseStatus";

    private void Awake()
    {
        SimcoachLoginUserManagement.OnUserSignedIn += CheckLicenseServer;
        SimcoachLoginUserManagement.OnUserSignedOut += Lock;
    }

    private void OnDestroy()
    {
        SimcoachLoginUserManagement.OnUserSignedIn -= CheckLicenseServer;
        SimcoachLoginUserManagement.OnUserSignedOut += Lock;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainMenuButtonsBlocker.SetActive(!SimcoachEnterpriseLicenseChecker.userAuthenticated);
        errorBlocker.SetActive(false);
    }

    /// <summary>
    /// codes:
    /// -1 = connected but OrganizationID does not exist in database.
    /// 0 = connected and data exists but "Active" is false.
    /// 1 = connected and data exists and marked as "Active".
    /// 400 = connection error.
    /// </summary>
    public void CheckLicenseServer()
    {
        if (SimcoachLoginUserManagement.GetIsOfflineMode())
            UpdateUI(true, PlayerPrefs.GetInt(offlineLicensePrefKey + SimcoachLoginUserManagement.GetActiveUserName(), 400), "Offline Mode");
        else  
            SimcoachEnterpriseLicenseChecker.Instance.GetLicenseStatus(UpdateUI);
    }

    void UpdateUI(bool success, int code, string message)
    {
        if (success)
        {
            if (code == -1)
            {
                GeneralErrorMessageBox.Show("Organization Not Recognized", error_noOrgID);
                errorBlockerText.text = error_noOrgID;
                errorBlocker.SetActive(true);
            }
            else if (code == 0)
            {
                GeneralErrorMessageBox.Show("Invalid License", error_inactiveOrgID);
                errorBlockerText.text = error_inactiveOrgID;
                errorBlocker.SetActive(true);
            }
            else if (code == 2)
            {
                GeneralErrorMessageBox.Show("Game Not Included In License", error_invalidGameID);
                errorBlockerText.text = error_invalidGameID;
                errorBlocker.SetActive(true);
            }
            else if (code == 1)
            {
                mainMenuButtonsBlocker.SetActive(false);
                SimcoachEnterpriseLicenseChecker.userAuthenticated = true;
            }
        }
        else
        {
            code = 400;
            GeneralErrorMessageBox.Show("Connectivity Issue", error_connectivityError);
            errorBlockerText.text = error_connectivityError + "\n\nNerd Stuff: " + message;
            errorBlocker.SetActive(true);
        }

        PlayerPrefs.SetInt(offlineLicensePrefKey + SimcoachLoginUserManagement.GetActiveUserName(), code);
    }

    public void Lock()
    {
        SimcoachEnterpriseLicenseChecker.userAuthenticated = false;
        mainMenuButtonsBlocker.SetActive(true);
        errorBlocker.SetActive(false);

        PlayerPrefs.DeleteKey(offlineLicensePrefKey + SimcoachLoginUserManagement.GetActiveUserName());
    }

    //use to store license status locally for offline mode
    public void AddLicense()
    {
        PlayerPrefs.SetInt(SimcoachLoginUserManagement.GetActiveUserName(), 1);
    }

    public void RemoveLicense()
    {
        PlayerPrefs.SetInt(SimcoachLoginUserManagement.GetActiveUserName(), 0);
    }

}
