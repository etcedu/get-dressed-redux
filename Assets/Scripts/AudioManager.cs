using System.Collections;
using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;
using Crosstales.RTVoice.Model;
using Crosstales.RTVoice.Model.Enum;
using UnityEngine.Events;
using Crosstales.RTVoice;

/*
 * WTP: There are really two halves of a system in one file here. The procuring of the audio and the playing of the audio.
 * If it is helpful in the future it might be a good idea to branch the file management side of this file into a dedicated
 * ResourceRequester class (or similar)
 */
public class AudioManager : MonoBehaviour
{    
    //Singleton
    public static AudioManager Instance { get; private set; }

    public int audioRepeatedNum { get; private set; }
    
    void Awake()
    {
        //Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void SpeakWithRTVoice(string text, string speaker, bool reverb = false)
    {
        if (reverb)
        {
            SimpleRTVoiceExample.Instance.voiceMixerGroup.audioMixer.SetFloat("ReverbDry", -800f);
            SimpleRTVoiceExample.Instance.voiceMixerGroup.audioMixer.SetFloat("ReverbRoom", -1000f);
        }
        else
        {
            SimpleRTVoiceExample.Instance.voiceMixerGroup.audioMixer.SetFloat("ReverbDry", 0);
            SimpleRTVoiceExample.Instance.voiceMixerGroup.audioMixer.SetFloat("ReverbRoom", -10000f);
        }

        SimpleRTVoiceExample.Instance.Speak(SimpleRTVoiceExample.Instance.GetVoice(speaker), text);
    }


    public void PausePlaying()
    {
        SimpleRTVoiceExample.Instance.PauseSpeech();
    }

    public void UnPausePlaying()
    {
        
    }

    public void StopPlaying()
    {
        SimpleRTVoiceExample.Instance.PauseSpeech();
    }
}
