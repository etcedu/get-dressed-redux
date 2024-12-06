using UnityEngine;
using System.Collections.Generic;

class TimeScaler : MonoBehaviour
{
    public float presetTimeScale = 1f;

    public void Preset()
    {
        Time.timeScale = presetTimeScale;
    }

    public void Normal()
    {
        Time.timeScale = 1f;
    }

    public void Zero()
    {
        Time.timeScale = 0;
    }

    public void Set(float scale)
    {
        Time.timeScale = scale;
    }

    public void SetAndRetain(float scale)
    {
        presetTimeScale = scale;
        Preset();
    }
}