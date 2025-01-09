using Simcoach.SkillArcade;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDetailPanel : MonoBehaviour
{
    [SerializeField] GameObject[] detailDescObjsToEnable;
    [SerializeField] GameObject[] detailDescObjsToDisable;
    [SerializeField] UITweener_SA[] tweens;
    [SerializeField] TMP_Text buttonLabel;

    bool detailVisible;

    private void Start()
    {
        HideDetail();
    }

    public void Toggle()
    {
        if (detailVisible)
            HideDetail();
        else
            ShowDetail();
    }

    public void ShowDetail()
    {
        detailVisible = true;
        for (int i = 0; i < detailDescObjsToEnable.Length; i++)
            detailDescObjsToEnable[i].SetActive(true);
        for (int i = 0; i < detailDescObjsToDisable.Length; i++)
            detailDescObjsToDisable[i].SetActive(false);
        for (int i = 0; i < tweens.Length; i++)
            tweens[i].PlayForward();

        buttonLabel.text = "BACK";
    }

    public void HideDetail()
    {
        detailVisible = false;
        for (int i = 0; i < detailDescObjsToEnable.Length; i++)
            detailDescObjsToEnable[i].SetActive(false);
        for (int i = 0; i < detailDescObjsToDisable.Length; i++)
            detailDescObjsToDisable[i].SetActive(true);
        for (int i = 0; i < tweens.Length; i++)
            tweens[i].PlayReverse();

        buttonLabel.text = "DETAIL...";
    }
}
