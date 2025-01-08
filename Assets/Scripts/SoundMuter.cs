using UnityEngine;
using System.Collections;

public class SoundMuter : MonoBehaviour
{
	public string muteCheck = "MuteSound";
	bool mute;
	UUIPlaySound[] sounds = new UUIPlaySound[0];
	static SoundMuter _instance;
	
	// Use this for initialization
	IEnumerator Start ()
	{ 
		yield return null;
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
			mute = GameBase.Bools.GetValue(muteCheck, false);
			sounds = GameObject.Find("Canvas").transform.GetComponentsInChildren<UUIPlaySound>(true);
			SetMute();
		}
		else if (_instance != this)
		{
			DestroyImmediate(this);
		}
	}
	
	IEnumerator OnLevelWasLoaded(int notImportant)
	{
		yield return null;
        sounds = GameObject.Find("Canvas").transform.GetComponentsInChildren<UUIPlaySound>(true);
        SetMute();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(_instance == this && GameBase.Bools.GetValue(muteCheck, false) != mute)
		{
			mute = !mute;
			SetMute();
		}
	}
	
	void SetMute()
	{
		Debug.Log("SetMute: " + mute);
		foreach (UUIPlaySound sound in sounds)
			sound.volume = mute ? 0f : 1f;
	}
	
	public static bool Muted
	{
		get{
			if(_instance == null) return false;
			return _instance.mute;
		}
	}
}

