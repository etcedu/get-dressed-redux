using Crosstales.RTVoice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeakLabel : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    public void Speak()
    {
        label.ForceMeshUpdate();
        string message = label.GetParsedText();
        SimpleRTVoiceExample.Instance.Speak("default", message);
    }
}
