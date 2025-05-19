using SimcoachGames.EventRecorder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DressingUI : MonoBehaviour
{
    [SerializeField] DressingManager dressingManager;
    [SerializeField] Animator uiAnimator;
    [SerializeField] GameObject dressingCanvasObject;

    [SerializeField] GameObject infoPanel;
    [SerializeField] TMP_Text nameLabel;
    [SerializeField] TMP_Text positionLabel;
    [SerializeField] TMP_Text descriptionLabel;

    [SerializeField] List<ClothingPieceSelectionToggle> headToggles;
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
        if (GlobalData.isTutorial)
            infoPanel.SetActive(false);

        CheckAndSetReadyButtonState();
        SetUpClothingButtons();
        init = true;

        //set bad head piece to start with
        headToggles.Find(x => x.clothingPiece == GlobalData.currentCharacterSelection.headPieces[GlobalData.currentCharacterSelection.headPieces.Count-1]).toggle.SetIsOnWithoutNotify(true);
        SetClothingPieceManual(GlobalData.currentCharacterSelection.headPieces[GlobalData.currentCharacterSelection.headPieces.Count - 1]);
    }

    public void EnableInfoPanel()
    {
        infoPanel.SetActive(true);
    }

    void SetUpClothingButtons()
    {
        for (int i = 0; i < headToggles.Count; i++)
            headToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < topToggles.Length; i++)
            topToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < bottomToggles.Length; i++)
            bottomToggles[i].gameObject.SetActive(false);
        for (int i = 0; i < feetToggles.Length; i++)
            feetToggles[i].gameObject.SetActive(false);


        for (int i = 0; i < headToggles.Count; i++)
        {
            if (i > GlobalData.currentCharacterSelection.headPieces.Count-1)
            {
                headToggles.RemoveAt(i);
            }
            else
            {
                headToggles[i].gameObject.SetActive(true);
                headToggles[i].InitButton(GlobalData.currentCharacterSelection.headPieces[i]);
            }
        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.topPieces.Count; i++)
        {
            topToggles[i].gameObject.SetActive(true);
            topToggles[i].InitButton(GlobalData.currentCharacterSelection.topPieces[i]);

        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.bottomPieces.Count; i++)
        {
            bottomToggles[i].gameObject.SetActive(true);
            bottomToggles[i].InitButton(GlobalData.currentCharacterSelection.bottomPieces[i]);
        }
        for (int i = 0; i < GlobalData.currentCharacterSelection.feetPieces.Count; i++)
        {
            feetToggles[i].gameObject.SetActive(true);
            feetToggles[i].InitButton(GlobalData.currentCharacterSelection.feetPieces[i]);
        }

        headToggles.Shuffle();
        for (int i = 0; i < headToggles.Count; i++)
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
                if (!headButtonsOpen && !GlobalData.isTutorial)
                    SimpleRTVoiceExample.Instance.Speak("default", "Head");

                uiAnimator.CrossFade(headButtonsOpen ? "CloseHeadButtons" : "OpenHeadButtons", 0.2f);
                uiAnimator.SetBool("HeadButtonsOpen", !headButtonsOpen);
                break;
            case Category.TOP:
                if (!topButtonsOpen && !GlobalData.isTutorial)
                    SimpleRTVoiceExample.Instance.Speak("default", "Top");

                uiAnimator.CrossFade(topButtonsOpen ? "CloseTopButtons" : "OpenTopButtons", 0.2f);
                uiAnimator.SetBool("TopButtonsOpen", !topButtonsOpen);
                break;
            case Category.BOTTOM:
                if (!bottomButtonsOpen && !GlobalData.isTutorial)
                    SimpleRTVoiceExample.Instance.Speak("default", "Bottom");

                uiAnimator.CrossFade(bottomButtonsOpen ? "CloseBottomButtons" : "OpenBottomButtons", 0.2f);
                uiAnimator.SetBool("BottomButtonsOpen", !bottomButtonsOpen);
                break;
            case Category.FEET:
                if (!feetButtonsOpen && !GlobalData.isTutorial)
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
        if (sender.clothingPiece.Category == Category.HEAD)
            dressingManager.PlayHairSounds();
        else
            dressingManager.PlayClothSound();
        
        if (!GlobalData.isTutorial)
            SimpleRTVoiceExample.Instance.Speak("default", sender.clothingPiece.DisplayName);

        CheckAndSetReadyButtonState();
    }

    void SetClothingPieceManual(ClothingPiece clothingPiece)
    {
        dressingManager.ClearClothingFromCategory(clothingPiece.Category);
        dressingManager.SetClothing(clothingPiece);
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
