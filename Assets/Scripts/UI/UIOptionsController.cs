using Crosstales;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOptionsController : MonoBehaviour
{
    [SerializeField] GameObject optionsPopup;
    [SerializeField] Toggle musicToggle;
    [SerializeField] UICreditsPanel creditsPanel;
    //[SerializeField] Toggle soundsToggle;

    int unlockTaps = 0;
    [SerializeField] GameObject unlockPopup;

    void OnEnable()
    {
        musicToggle.isOn = !MusicManager.MusicIsOn;
        // soundsToggle.isOn = !SFXManager.SFXIsOn;
    }

    // void Start()
    // {
    //     CloseOptions();
    // }

    public void OpenOptions()
    {
        optionsPopup.SetActive(true);
        //EventRecorder.LogEvent_OpenedSettings();
    }

    public void CloseOptions()
    {
        optionsPopup.SetActive(false);
    }

    public void MainMenu()
    {
        //EventRecorder.LogEvent_QuitLevel(
        //    GlobalData.GetDifficultyName(),
        //    InterviewController.Instance.realInterviewTime,
        //    Mathf.RoundToInt(GlobalData.scoreSystem.behaviorPointsAwarded),
        //    Mathf.RoundToInt(GlobalData.scoreSystem.answerPointsAwarded),
        //    InterviewController.Instance.questionsAnswered);
        
        SceneLoader.LoadScene("MainMenu");
    }

    public void ClearData()
    {
        ApplicationManager.ResetData();
    }

    public void UnlockGame()
    {
        if (unlockTaps == 3)
        {   
            ApplicationManager.UnlockGame();
            unlockPopup.SetActive(true);
        }
        else
        {
            unlockTaps++;
        }
    }

    public void Credits()
    {
        creditsPanel.Button_Open();
    }

    public void MusicToggled(bool isMuted)
    {
        if (isMuted)
        {
            MusicManager.ToggleMusicOff();
        }
        else
        {
            MusicManager.ToggleMusicOn();
        }
    }

    public void SFXToggled(bool isMuted)
    {
        if (isMuted)
        {
            SFXManager.ToggleSFXOff();
        }
        else
        {
            SFXManager.ToggleSFXOn();
        }
        
    }
}
