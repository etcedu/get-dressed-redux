using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] SoundVolumePair volumePair;
    SFXManager sfxManager;

    public void Play()
    {
        if (sfxManager == null)
            sfxManager = FindObjectOfType<SFXManager>();

        if (volumePair != null)
            sfxManager?.PlayOneShot(volumePair);
        else
            sfxManager?.PlayOneShot(clip);

    }
}
