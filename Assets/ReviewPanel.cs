using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReviewPanel : MonoBehaviour
{
    public UnityEvent openEvents;
    public UnityEvent closeEvents;
    public bool isOpen;

    public void OnButton()
    {
        if (!isOpen)
            Open();
        else
            Close();
    }

    public void Open()
    {
        if (isOpen) return;
        openEvents.Invoke();
        isOpen = true;
    }

    public void Close()
    {
        if (!isOpen) return;
        SimpleRTVoiceExample.Instance.StopSpeech();
        closeEvents.Invoke();
        isOpen = false;
    }
}
