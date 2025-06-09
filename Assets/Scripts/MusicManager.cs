using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    //Note that this is not static. It's possible more than one of these components exsists in the scene.
    AudioSource audioSource;

    [SerializeField] bool useAlternateMusic;
    [SerializeField] SoundVolumePair[] musicTrackSoundVolumePairs;
    [SerializeField] SoundVolumePair[] musicTrackSoundVolumePairsAlternate;


    float trackVolume;

    bool init = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0;

        SoundVolumePair track = useAlternateMusic ? musicTrackSoundVolumePairsAlternate[UnityEngine.Random.Range(0, musicTrackSoundVolumePairsAlternate.Length)] 
                                                  : musicTrackSoundVolumePairs[UnityEngine.Random.Range(0, musicTrackSoundVolumePairs.Length)];
        audioSource.clip = track.clip;
        trackVolume = track.volume;
        audioSource.Play();

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
                
        StartCoroutine(fadeInMusic());
        DontDestroyOnLoad(this.gameObject);
        init = true;
    }   

    void SettingChanged(bool sfxIsOn)
    {
        audioSource.mute = !sfxIsOn;
    }

    public void ChangeMusic(SoundVolumePair svp)
    {
        trackVolume = svp.volume;
        ChangeMusic(svp.clip);
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

    public void FadeOutMusic()
    {
        StartCoroutine(waitForInit(() =>
        {
            StopAllCoroutines();
            StartCoroutine(fadeOutMusic());
        }));
    }

    IEnumerator waitForInit(Action actionAfterInit)
    {
        while (!init)
            yield return null;

        actionAfterInit.Invoke();
    }

    IEnumerator fadeInMusic()
    {
        while (audioSource.volume < trackVolume)
        {
            audioSource.volume += Time.deltaTime * 0.75f;
            yield return null;
        }
        audioSource.volume = trackVolume;
    }

    IEnumerator fadeOutMusic()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.75f;
            yield return null;
        }
    }

    IEnumerator fadeOutMusicAndKill()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.75f;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}