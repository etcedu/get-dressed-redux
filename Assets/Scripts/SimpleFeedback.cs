using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BehaviorFeedback
{
    public List<string> details = new List<string>();
    public List<string> buttonLabel = new List<string>();
}

public class SimpleFeedback : MonoBehaviour
{
    [SerializeField] GameObject feedbackCanvasObject;

    [SerializeField] Animator animator;
    [SerializeField] Animator charAnimator;

    [Header("Intro Panel")]
    [SerializeField] string fitTitle, okTitle, unfitTitle;
    [SerializeField] TMP_Text fitOrNotHeader, fitOrNotText;


    [Header("Feedback Details")]
    [SerializeField] GameObject[] stars;
    [SerializeField] Image starBarFill;
    [SerializeField] List<TMP_Text> feedbackClothingName;
    [SerializeField] List<TMP_Text> feedbackTexts;
    [SerializeField] List<Image> feedbackButtonFaces;
    [SerializeField] List<TMP_Text> feedbackButtonTexts;
    [SerializeField] Button bottomButton;
    [SerializeField] string[] feedbackButtonLabelOptions;
    [SerializeField] Image[] headerImages;

    [Header("General")]    
    [SerializeField] List<Color> scoreColors;

    [SerializeField] SoundVolumePair fitMusic, unfitMusic;
    [SerializeField] MusicManager musicManager;
    [SerializeField] SoundVolumePair feedbackDrumroll_Good, feedbackDrumroll_Bad;
    bool fit;
    float gameDuration;

    bool didStarFillAnimation;

    private void Start()
    {
        feedbackCanvasObject.SetActive(false);
    }

    public void StartFeedback(double duration)
    {
        gameDuration = (float)duration;
        SetupTotals();
        StartCoroutine(startFeedbackRoutine());
    }

    IEnumerator startFeedbackRoutine()
    {
        SFXManager.instance.PlayOneShot(fit ? feedbackDrumroll_Good : feedbackDrumroll_Bad);
        animator.Play("MoveToFeedbackPos");
        yield return new WaitForSeconds(1.8f);
        feedbackCanvasObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(starsFillRoutine());
        
        if (GlobalData.isTutorial)
        {
            SimpleRTVoiceExample.Instance.Speak("default", $"{fitOrNotHeader.text}, {fitOrNotText.text}");
        }
    }


    IEnumerator starsFillRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        didStarFillAnimation = true;
        float percentageScore = GlobalData.GetOverallScore();
        Debug.Log($"Percentage score: {percentageScore}");

        while (starBarFill.fillAmount < percentageScore)
        {
            starBarFill.fillAmount += Time.deltaTime * 0.6f;
            if (starBarFill.fillAmount > percentageScore)
                starBarFill.fillAmount = percentageScore;

            if (starBarFill.fillAmount >= 0.25f)
                stars[0].SetActive(true);
            if (starBarFill.fillAmount >= 0.50f)
                stars[1].SetActive(true);
            if (starBarFill.fillAmount >= 0.75f)
                stars[2].SetActive(true);
            if (starBarFill.fillAmount >= 0.99f)
                stars[3].SetActive(true);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    void SetupTotals()
    {
        float scorePercentage = GlobalData.GetOverallScore();
        fit = scorePercentage >= 1.0f;

        if (GlobalData.isTutorial && fit)
            GlobalData.SetTutorialState(true);
        GlobalData.SetCharacterCompleted(GlobalData.currentCharacterSelection.characterTag, fit);

        starBarFill.fillAmount = 0;

        fitOrNotHeader.text = scorePercentage >= 0.99f ? fitTitle
                            : (scorePercentage >= 0.75f ? okTitle 
                            : unfitTitle);

        fitOrNotText.text = scorePercentage >= 0.99f ? GlobalData.currentCharacterSelection.winFeedback 
                         : (scorePercentage >= 0.75f ? GlobalData.currentCharacterSelection.okFeedback 
                         : GlobalData.currentCharacterSelection.loseFeedback);

        foreach (Image i in headerImages)
            i.color = scorePercentage >= 0.99f ? scoreColors[2]
                    : (scorePercentage >= 0.75f ? scoreColors[1] 
                    : scoreColors[0]);

        fitOrNotHeader.ForceMeshUpdate();
        fitOrNotText.ForceMeshUpdate();
        
        musicManager.ChangeMusic(fit ? fitMusic : unfitMusic);

        foreach (ClothingPiece clothingPiece in GlobalData.GetListOfSelectedClothes())
        {
            if (clothingPiece.Category == Category.DRESS)
                bottomButton.interactable = false;

            int score = GlobalData.GetScoreForPiece(clothingPiece);

            Debug.Log($"Checking: {clothingPiece.Category}  Score: {score}");

            int uiIndex = clothingPiece.Category == Category.HEAD ? 0 : (clothingPiece.Category == Category.TOP || clothingPiece.Category == Category.DRESS ? 1 : (clothingPiece.Category == Category.BOTTOM ? 2 : 3));
            feedbackClothingName[uiIndex].text = clothingPiece.FeedbackName;
            feedbackTexts[uiIndex].text = clothingPiece.Feedback;
            feedbackButtonTexts[uiIndex].text = feedbackButtonLabelOptions[score - 1];
            feedbackButtonFaces[uiIndex].color = scoreColors[score - 1];
        }

        EventRecorder.RecordLevelCompletedEvent(gameDuration, fit, scorePercentage, GlobalData.currentCharacterSelection.characterName,
            GlobalData.selectedHeadPiece.DisplayName, GlobalData.GetScoreForPiece(GlobalData.selectedHeadPiece),
            GlobalData.selectedTopPiece.DisplayName, GlobalData.GetScoreForPiece(GlobalData.selectedTopPiece),
            GlobalData.selectedBottomPiece.DisplayName, GlobalData.GetScoreForPiece(GlobalData.selectedBottomPiece),
            GlobalData.selectedFeetPiece.DisplayName, GlobalData.GetScoreForPiece(GlobalData.selectedFeetPiece));
    }



    public void LoadMainMenu()
    {
        SimpleRTVoiceExample.Instance.StopSpeech();
        SceneLoader.LoadScene("LevelSelection");
    }

    public void ReadFeedback(TMP_Text textObject)
    {
        textObject.ForceMeshUpdate();
        string message = textObject.GetParsedText();
        AudioManager.Instance.SpeakWithRTVoice(message, "feedbackVoice");
    }

    public void SpeakMainFeedback()
    {
        fitOrNotHeader.ForceMeshUpdate();
        fitOrNotText.ForceMeshUpdate();
        string message = $"{fitOrNotHeader.text}, {fitOrNotText.text}";
        SimpleRTVoiceExample.Instance.Speak("default", message);
    }

    public void ClickedReviewFeedbackButton()
    {
        //EventRecorder.RecordViewedDetailedFeedbackEvent();
    }
}
