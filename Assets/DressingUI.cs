using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressingUI : MonoBehaviour
{
    [SerializeField] Animator uiAnimator;


    public void ClothingCategoryButton_OnClick(string category)
    {
        Enum.TryParse(category, out Category categoryParsed);
        Debug.Log(categoryParsed);

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
                uiAnimator.CrossFade(headButtonsOpen ? "CloseHeadButtons" : "OpenHeadButtons", 0.2f);
                uiAnimator.SetBool("HeadButtonsOpen", !headButtonsOpen);
                break;
            case Category.TOP:
                uiAnimator.CrossFade(topButtonsOpen ? "CloseTopButtons" : "OpenTopButtons", 0.2f);
                uiAnimator.SetBool("TopButtonsOpen", !topButtonsOpen);
                break;
            case Category.BOTTOM:
                uiAnimator.CrossFade(bottomButtonsOpen ? "CloseBottomButtons" : "OpenBottomButtons", 0.2f);
                uiAnimator.SetBool("BottomButtonsOpen", !bottomButtonsOpen);
                break;
            case Category.SHOES:
                uiAnimator.CrossFade(feetButtonsOpen ? "CloseFeetButtons" : "OpenFeetButtons", 0.2f);
                uiAnimator.SetBool("FeetButtonsOpen", !feetButtonsOpen);
                break;
            default:
                break;
        }
    }
}
