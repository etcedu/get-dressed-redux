using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    
    public static bool SFXIsOn { get; private set; } = true;
    public static Action<bool> OnSFXSettingChanged;

    //Note that this is not static. It's possible more than one of these components exsists in the scene.
    AudioSource audioSource;

    [Header("Sounds")] 
    [SerializeField] AudioClip buttonClick;

    //Must be created and assigned manually
    [SerializeField] Slider mainVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider ambienceVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] Slider voiceVolumeSlider;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (SFXIsOn)
        {
            ToggleSFXOn();
        }
        else
        {
            ToggleSFXOff();
        }
        
        //Update instances
        OnSFXSettingChanged += SettingChanged;
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        float mainVolume = PlayerPrefs.GetFloat("MainVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1f);

        FindSliders();

        if (mainVolumeSlider != null)
            mainVolumeSlider.value = mainVolume;
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        if (ambienceVolumeSlider != null)
            ambienceVolumeSlider.value = ambienceVolume;
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
        if (voiceVolumeSlider != null)
            voiceVolumeSlider.value = voiceVolume;
    }

    //this is a suboptimal solution to finding the inactive volume sliders
    void FindSliders()
    {
        List<Slider> sliders = GameObject.FindObjectsOfType<Slider>(true).ToList();
        mainVolumeSlider = sliders.Find(x => x.gameObject.name == "MainVolumeSlider");
        musicVolumeSlider = sliders.Find(x => x.gameObject.name == "MusicVolumeSlider");
        ambienceVolumeSlider = sliders.Find(x => x.gameObject.name == "AmbienceVolumeSlider");
        sfxVolumeSlider = sliders.Find(x => x.gameObject.name == "SFXVolumeSlider");
        voiceVolumeSlider = sliders.Find(x => x.gameObject.name == "VoiceVolumeSlider");

        if (mainVolumeSlider != null) mainVolumeSlider.onValueChanged.AddListener(SetMainVolume);
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (ambienceVolumeSlider != null) ambienceVolumeSlider.onValueChanged.AddListener(SetAmbienceVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (voiceVolumeSlider != null) voiceVolumeSlider.onValueChanged.AddListener(SetVoiceVolume);
    }

    public static void ToggleSFXOn()
    {
        SFXIsOn = true;
        OnSFXSettingChanged?.Invoke(SFXIsOn);
    }

    public static void ToggleSFXOff()
    {
        SFXIsOn = false;
        OnSFXSettingChanged?.Invoke(SFXIsOn);
    }

    void SettingChanged(bool sfxIsOn)
    {
        audioSource.mute = !sfxIsOn;
    }

    void OnDestroy()
    {
        OnSFXSettingChanged -= SettingChanged;
    }
    
    public void PlayButtonSound()
    {
        if (!SFXIsOn) return;

        if (audioSource == null) return;//audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(buttonClick);
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        if (!SFXIsOn || audioClip == null)
            return;

        audioSource.PlayOneShot(audioClip);
    }

    #region DynamicFunctionsForSliders
    public void SetMainVolume(float soundLevel)
    {
        audioMixer.SetFloat("MainVolume", Mathf.Log(soundLevel) * 20);
        PlayerPrefs.SetFloat("MainVolume", soundLevel);
    }

    public void SetMusicVolume(float soundLevel)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log(soundLevel) * 20);
        PlayerPrefs.SetFloat("MusicVolume", soundLevel);
    }

    public void SetAmbienceVolume(float soundLevel)
    {
        audioMixer.SetFloat("AmbienceVolume", Mathf.Log(soundLevel) * 20);
        PlayerPrefs.SetFloat("AmbienceVolume", soundLevel);
    }

    public void SetSFXVolume(float soundLevel)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log(soundLevel) * 20);
        PlayerPrefs.SetFloat("SFXVolume", soundLevel);
    }
    public void SetVoiceVolume(float soundLevel)
    {
        audioMixer.SetFloat("VoiceVolume", Mathf.Log(soundLevel) * 20);
        PlayerPrefs.SetFloat("VoiceVolume", soundLevel);
    }
    #endregion
}
