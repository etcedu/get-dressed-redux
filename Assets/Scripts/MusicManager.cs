using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static bool MusicIsOn { get; private set; } = true;
    public static Action<bool> OnMusicSettingChanged;

    //Note that this is not static. It's possible more than one of these components exsists in the scene.
    AudioSource audioSource;

    bool init = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;
        Debug.Log($"{audioSource.clip.name}");

        List<MusicManager> otherMusicSources = FindObjectsOfType<MusicManager>().ToList();
        otherMusicSources.Remove(this);
        if (otherMusicSources?.Count > 0)
        {
            foreach (MusicManager otherMusic in otherMusicSources)
            {
                if (otherMusic.audioSource.clip != audioSource.clip)
                    StartCoroutine(otherMusic.fadeOutMusicAndKill());
                else
                    Destroy(gameObject);
            }
        }

        //Update instances
        OnMusicSettingChanged += SettingChanged; 
        
        if (MusicIsOn)
        {
            ToggleMusicOn();
        }
        else
        {
            ToggleMusicOff();
        }

        StartCoroutine(fadeInMusic());
        DontDestroyOnLoad(this.gameObject);
        init = true;
    }

    public static void ToggleMusicOn()
    {
        MusicIsOn = true;
        OnMusicSettingChanged?.Invoke(MusicIsOn);
    }

    public static void ToggleMusicOff()
    {
        MusicIsOn = false;
        OnMusicSettingChanged?.Invoke(MusicIsOn);
    }

    void SettingChanged(bool sfxIsOn)
    {
        audioSource.mute = !sfxIsOn;
    }

    void OnDestroy()
    {
        OnMusicSettingChanged -= SettingChanged;
    }

    public void ChangeMusic(AudioClip clip)
    {
        StartCoroutine(waitForInit(() =>
        {
            StopAllCoroutines();
            audioSource.clip = clip;
            audioSource.volume = 0;
            audioSource.Play();
            StartCoroutine(fadeInMusic());
        }));
    }

    IEnumerator waitForInit(Action actionAfterInit)
    {
        while (!init)
            yield return null;

        actionAfterInit.Invoke();
    }

    public IEnumerator fadeInMusic()
    {
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime * 0.75f;
            yield return null;
        }
        audioSource.volume = 1;
    }

    public IEnumerator fadeOutMusicAndKill()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.75f;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}