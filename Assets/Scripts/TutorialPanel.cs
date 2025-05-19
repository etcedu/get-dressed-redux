using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    UITweener tween;

    void Start()
    {
        tween = GetComponent<UITweener>();
    }

    public void Show(string message)
    {
        gameObject.SetActive(true);
        text.text = message;
        SimpleRTVoiceExample.Instance.Speak("default", message);
        tween?.PlayForward();
    }

    public void Hide()
    {
        tween?.PlayReverse();
    }
}
