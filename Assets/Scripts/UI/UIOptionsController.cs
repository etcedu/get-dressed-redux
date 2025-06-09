using Crosstales;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOptionsController : MonoBehaviour
{
    [SerializeField] GameObject optionsPopup;
    [SerializeField] UICreditsPanel creditsPanel;

    int unlockTaps = 0;
    [SerializeField] GameObject unlockPopup;

    void OnEnable()
    {
       
    }

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
        GlobalData.ClearData();
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
}
