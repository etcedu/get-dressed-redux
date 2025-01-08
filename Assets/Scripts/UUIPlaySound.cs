//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Plays the specified sound.
/// </summary>

public class UUIPlaySound : MonoBehaviour
{

    public string muteCheck = "MuteSound";
    public AudioClip audioClip;

	[Range(0f, 1f)] public float volume = 1f;
	[Range(0f, 2f)] public float pitch = 1f;

	Button button;

	protected bool canPlay
	{
		get
		{
			if (!enabled) return false;
			if (button == null) button = GetComponent<Button>();
			return (button != null || button.interactable);
		}
	}

    private void Start()
    {
		button = GetComponent<Button>();
		button?.onClick.AddListener(() => Play());
    }

    public virtual void Play ()
	{
        if (!GameBase.Bools.GetValue(muteCheck, false))
			FindObjectOfType<SoundSource>().PlaySound(audioClip, volume, pitch);
	}
}
