using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundEffect : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    SFXManager sfxManager;

    private void Start()
    {
        sfxManager = FindObjectOfType<SFXManager>();
    }

    public void Play()
    {
        sfxManager = FindObjectOfType<SFXManager>();
        if (sfxManager != null)
            sfxManager.PlayOneShot(clip);
    }
}
