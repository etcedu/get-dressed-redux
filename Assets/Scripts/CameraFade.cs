using FancyScrollView.TheFitCharacterSelect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public static CameraFade Instance { get; private set; }
    
    TweenAlpha tween;
    Image image;
    CanvasGroup canvasGroup;

    public Action fadeFinishedCallback;

    IEnumerator Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        tween = GetComponent<TweenAlpha>();
        image = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;

        TheFitScrollView scrollView = FindObjectOfType<TheFitScrollView>();
        if (SceneManager.GetActiveScene().name == "LevelSelection")
        {
            while (!scrollView.init)
                yield return null;
        }
        yield return null;

        FadeIn();
    }

    [ContextMenu("Fade In")]
    public void FadeIn(Action finishedCallback = null)
    {
        //image.raycastTarget = false;

        fadeFinishedCallback = finishedCallback;
        Invoke("DoCallback", tween.duration + 0.25f);

        tween.@from = 1;
        tween.to = 0;
        tween.delay = 1.0f; //2f;
        tween.PlayForward_FromBeginning();
    }

    [ContextMenu("Fade Out")]
    public void FadeOut(Action finishedCallback = null)
    {
        //image.raycastTarget = true;

        fadeFinishedCallback = finishedCallback;
        Invoke("DoCallback", tween.duration + 0.25f);

        tween.@from = 0;
        tween.to = 1;
        tween.delay = 0.0f; //0.5f;
        tween.PlayForward_FromBeginning();
    }

    public void DoCallback()
    {
        fadeFinishedCallback?.Invoke();
        //image.raycastTarget = false;
    }
    
    
}
