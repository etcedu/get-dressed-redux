using System.Collections.Generic;
using UnityEngine;
using Crosstales.RTVoice;
using Crosstales.RTVoice.Model;
using Crosstales.RTVoice.Model.Enum;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Collections;


//WTP NOTE: Our game appears to rely on this modified version of the original example script from RT voice. Any weirdness is because
//we should never have used this file

/// <summary>
/// Simple example to demonstrate the basic usage of RT-Voice.
/// </summary>
/// 

[Serializable]
public class CharacterVoice
{
    [SerializeField] public string name;
    [SerializeField] public Gender gender;
    [SerializeField] public string culture;  
}

public class SimpleRTVoiceExample : MonoBehaviour
{
    [SerializeField] float onSpeakCompleteTriggerEarlyTimeReduction = 1.0f;
    [SerializeField] List<CharacterVoice> characterVoices = new();

    string text = "Hello world!";
 
    //Used to hack RTVoice
    public UnityEngine.Audio.AudioMixerGroup voiceMixerGroup;
    public Action onSpeakStartCallback;
    public Action onSpeakCompleteCallback;

    string uid; //Unique id of the speech

    int currentVoiceIndex = 0;

    static SimpleRTVoiceExample _instance;
    static Dictionary<string, Voice> voiceCache = new Dictionary<string, Voice>();
    static float ttsRate;

    public static SimpleRTVoiceExample Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void OnEnable()
    {
        // Subscribe event listeners
        Speaker.Instance.OnReady.AddListener(voicesReady);
        Speaker.Instance.OnSpeakStart += speakStart;
        Speaker.Instance.OnSpeakComplete += speakComplete;

        ttsRate = PlayerPrefs.GetFloat("ttsRate", 1.0f);
        FindObjectsOfType<Slider>(true).ToList().Find(x => x.gameObject.name == "TTSSpeedSlider")?.SetValueWithoutNotify(ttsRate);
    }

    void OnDisable()
    {
        if (Speaker.Instance != null)
        {
            // Unsubscribe event listeners
            Speaker.Instance.OnReady.RemoveListener(voicesReady);
            Speaker.Instance.OnSpeakStart -= speakStart;
            Speaker.Instance.OnSpeakComplete -= speakComplete;   
        }
    }

    public Voice NewRandomVoice(Gender gender)
    {
        int index = UnityEngine.Random.Range(0, Speaker.Instance.VoicesForGender(gender, "en").Count);
        return Speaker.Instance.VoiceForGender(gender, "en-US", index: index, isFuzzy: true);
    }

    public Voice GetVoice(string speaker)
    {
        if (voiceCache.ContainsKey(speaker))
            return voiceCache[speaker];

        CharacterVoice charVoice = characterVoices.Where((x) => x.name == speaker).FirstOrDefault();
        if (charVoice == null)
        {
            Debug.LogWarning($"No character voice found for {speaker}, using defaults");
            charVoice = new CharacterVoice();
            charVoice.gender = Gender.MALE;
            charVoice.culture = "en-US";
        }

        voiceCache[speaker] = Speaker.Instance.VoiceForGender(charVoice.gender, charVoice.culture, isFuzzy: true);
        return voiceCache[speaker];
    }

    public static void TestVoicesForGender(Gender gender)
    {
        List<Voice> voices = Speaker.Instance.VoicesForGender(gender);
        foreach (Voice voice in voices)
        {
            print(gender + " " + voice.Name);
        }        
    }

    public void Speak(Voice voice, string message)
    {
        message = message.Trim();
        message = message.Replace("<b>", "");
        message = message.Replace("</b>", "");
        message = message.Replace("<i>", "");
        message = message.Replace("</i>", "");
        message = message.Replace("<u>", "");
        message = message.Replace("</u>", "");
        message = message.Replace("<br>", "");
        Speaker.Instance.Silence();
        text = message;
        uid = Speaker.Instance.Speak(text, null, voice, rate: ttsRate);

        StartSpeakCompleteTimer(Speaker.Instance.ApproximateSpeechLength(text, rate: ttsRate) - onSpeakCompleteTriggerEarlyTimeReduction);
    }

    public void PauseSpeech()
    {
        Speaker.Instance.Pause();
    }

    public void UnpauseSpeech()
    {
        Speaker.Instance.UnPause();
    }

    public bool IsSpeaking()
    {
        return Speaker.Instance.isSpeaking;
    }

    public void StopSpeech()
    {
        Speaker.Instance.Silence();
    }

    public void ChangeTTSRate(float value)
    {
        PlayerPrefs.SetFloat("ttsRate", value);
        ttsRate = value;
    }

    void voicesReady()
    {
       // Debug.Log($"RT-Voice: {Speaker.Instance.Voices.Count} voices are ready to use!");
        TestVoicesForGender(Gender.MALE);
        TestVoicesForGender(Gender.FEMALE);
        TestVoicesForGender(Gender.UNKNOWN);


        //WTP TODO: Why are we doing this? In case something has already been queued up? I added
        //the gender param to speak so... I guess this one will be unknown?
        // if (SpeakWhenReady) //Speak after the voices are ready
        //     Speak(Gender.UNKNOWN);
    }

    void speakStart(Wrapper wrapper)
    {
        if (wrapper.Uid == uid) //Only write the log message if it's "our" speech
        {
           // Debug.Log($"RT-Voice: speak started: {wrapper}");
            onSpeakStartCallback?.Invoke();
        }
    }

    void speakComplete(Wrapper wrapper)
    {
        if (wrapper.Uid == uid) //Only write the log message if it's "our" speech
        {
            // Debug.Log($"RT-Voice: speak completed: {wrapper}");
            onSpeakCompleteCallback?.Invoke();
            onSpeakCompleteCallback = null;
        }
    }

    public void StartSpeakCompleteTimer(float delay)
    {
        //Debug.Log($"------ StartSpeakCompleteTimer -- {delay}");
        StartCoroutine(invokeSpeakCompleteOnTimer(delay));
    }

    IEnumerator invokeSpeakCompleteOnTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        onSpeakCompleteCallback?.Invoke();
        onSpeakCompleteCallback = null;
    }

    public void StopSpeakComplete()
    {
        StopAllCoroutines();
        onSpeakCompleteCallback = null;
    }
}
// © 2022 crosstales LLC (https://www.crosstales.com)