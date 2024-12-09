using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {            
        source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float pitch, float volume)
    {
        if (source == null)
            source = GetComponent<AudioSource>();
        source.pitch = pitch;
        source.PlayOneShot(clip, volume);
    }
}
