using SimcoachGames.EventRecorder;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DressingUI : MonoBehaviour
{
    [SerializeField] DressingManager dressingManager;
    [SerializeField] Animator uiAnimator;
    [SerializeField] GameObject dressingCanvasObject;

    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text positionLabel;
    [SerializeField] TMP_Text descriptionLabel;

    [SerializeField] ClothingPieceSelectionToggle[] headToggles;
    [SerializeField] ClothingPieceSelectionToggle[] topToggles;
    [SerializeField] ClothingPieceSelectionToggle[] bottomToggles;
    [SerializeField] ClothingPieceSelectionToggle[] feetToggles;

    [SerializeField] Button readyButton;

    bool init;

    private IEnumerator Start()
    {
        //triggers all the bouncing intro tweens
        dressingCanvasObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        dressingCanvasObject.SetActive(true);
    }

    public void Init()
    {
        nameLabel.text = GlobalData.currentCharacterSelection.characterName;
        positionLabel.text = GlobalData.currentCharacterSelection.jobTitle;
        descriptionLabel.text = GlobalData.currentCharacterSelection.description;

        CheckAndSetReadyButtonState();
        SetUpClothingButtons();
        init = true;
    }

    void SetUpClothingButtons()
    {
        for (int i = 0; i < headToggles.Length; i++)
            headToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < topToggles.Length; i++)
            topToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < bottomToggles.Length; i++)
            bottomToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < feetToggles.Length; i++)
            feetToggles[i].gameObject.SetActive(false);


        for (int i = 0; i < GlobalData.currentCharacterSelection.headOptions.Length; i++)
        {
            headToggles[i].gameObject.SetActive(true);
            headToggles[i].InitButton(GlobalData.GetPieceOfClothing(GlobalData.currentCharacterSelection.headOptions[i]));
        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.topOptions.Length; i++)
        {
            topToggles[i].InitButton(GlobalData.GetPieceOfClothing(GlobalData.currentCharacterSelection.topOptions[i]));
            topToggles[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.bottomOptions.Length; i++)
        {
            bottomToggles[i].InitButton(GlobalData.GetPieceOfClothing(GlobalData.currentCharacterSelection.bottomOptions[i]));
            bottomToggles[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.feetOptions.Length; i++)
        {
            feetToggles[i].InitButton(GlobalData.GetPieceOfClothing(GlobalData.currentCharacterSelection.feetOptions[i]));
            feetToggles[i].gameObject.SetActive(true);
        }

        headToggles.Shuffle();
        for (int i = 0; i < headToggles.Length; i++)
            headToggles[i].transform.SetSiblingIndex(i);
        topToggles.Shuffle();
        for (int i = 0; i < topToggles.Length; i++)
            topToggles[i].transform.SetSiblingIndex(i);
        bottomToggles.Shuffle();
        for (int i = 0; i < bottomToggles.Length; i++)
            bottomToggles[i].transform.SetSiblingIndex(i);
        feetToggles.Shuffle();
        for (int i = 0; i < feetToggles.Length; i++)
            feetToggles[i].transform.SetSiblingIndex(i);
    }

    public void ClothingCategoryButton_OnClick(string category)
    {
        Enum.TryParse(category, out Category categoryParsed);

        bool headButtonsOpen    = uiAnimator.GetBool("HeadButtonsOpen");
        bool topButtonsOpen     = uiAnimator.GetBool("TopButtonsOpen");
        bool bottomButtonsOpen  = uiAnimator.GetBool("BottomButtonsOpen");
        bool feetButtonsOpen    = uiAnimator.GetBool("FeetButtonsOpen");

        if (headButtonsOpen)    { uiAnimator.CrossFade("CloseHeadButtons", 0.2f);   headButtonsOpen = false; }
        if (topButtonsOpen)     { uiAnimator.CrossFade("CloseTopButtons", 0.2f);    topButtonsOpen = false; }
        if (bottomButtonsOpen)  { uiAnimator.CrossFade("CloseBottomButtons", 0.2f); bottomButtonsOpen = false; }
        if (feetButtonsOpen)    { uiAnimator.CrossFade("CloseFeetButtons", 0.2f);   feetButtonsOpen = false; }

        switch (categoryParsed)
        {
            case Category.HEAD:
                if (!headButtonsOpen)
                    SimpleRTVoiceExample.Instance.Speak("default", "Head");

                uiAnimator.CrossFade(headButtonsOpen ? "CloseHeadButtons" : "OpenHeadButtons", 0.2f);
                uiAnimator.SetBool("HeadButtonsOpen", !headButtonsOpen);
                break;
            case Category.TOP:
                if (!topButtonsOpen)
                    SimpleRTVoiceExample.Instance.Speak("default", "Top");

                uiAnimator.CrossFade(topButtonsOpen ? "CloseTopButtons" : "OpenTopButtons", 0.2f);
                uiAnimator.SetBool("TopButtonsOpen", !topButtonsOpen);
                break;
            case Category.BOTTOM:
                if (!bottomButtonsOpen)
                    SimpleRTVoiceExample.Instance.Speak("default", "Bottom");

                uiAnimator.CrossFade(bottomButtonsOpen ? "CloseBottomButtons" : "OpenBottomButtons", 0.2f);
                uiAnimator.SetBool("BottomButtonsOpen", !bottomButtonsOpen);
                break;
            case Category.FEET:
                if (!feetButtonsOpen)
                    SimpleRTVoiceExample.Instance.Speak("default", "Feet");

                uiAnimator.CrossFade(feetButtonsOpen ? "CloseFeetButtons" : "OpenFeetButtons", 0.2f);
                uiAnimator.SetBool("FeetButtonsOpen", !feetButtonsOpen);
                break;
            default:
                break;
        }
    }

    public void ClothingToggle_OnClick(ClothingPieceSelectionToggle sender)
    {
        if (!init)
            return;

        dressingManager.ClearClothingFromCategory(sender.clothingPiece.Category);
        dressingManager.SetClothing(sender.clothingPiece);
        SimpleRTVoiceExample.Instance.Speak("default", sender.clothingPiece.DisplayName);

        CheckAndSetReadyButtonState();
    }

    void CheckAndSetReadyButtonState()
    {
        readyButton.interactable = GlobalData.selectedBottomPiece != null &&
            GlobalData.selectedFeetPiece != null &&
            GlobalData.selectedTopPiece != null &&
            GlobalData.selectedHeadPiece != null;
    }

    public void Hide()
    {
        dressingCanvasObject.SetActive(false);
    }
}
