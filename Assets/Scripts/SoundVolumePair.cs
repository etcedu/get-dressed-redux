using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/SoundVolumePair", fileName = "New SoundVolumePair")]
[System.Serializable]
public class SoundVolumePair : ScriptableObject
{
    public AudioClip clip;
    [Range(0.0f, 2.0f)]
    public float volume;
}
