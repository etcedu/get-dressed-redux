using UnityEngine;
using System.Collections;

public class SoundMuter : MonoBehaviour
{
	public string muteCheck = "MuteSound";
	bool mute;
	UIPlaySound[] sounds = new UIPlaySound[0];
	static SoundMuter _instance;
	
	// Use this for initialization
	IEnumerator Start ()
	{
		if(Application.isLoadingLevel)
			yield return null;
		yield return null;
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
			mute = GameBase.Bools.GetValue(muteCheck, false);
			sounds = GameObject.FindObjectsOfType<UIPlaySound>();
			SetMute();
		} else if(_instance != this)
		{
			DestroyImmediate(this);
		}
	}
	
	IEnumerator OnLevelWasLoaded(int notImportant)
	{
		if(Application.isLoadingLevel)
			yield return null;
		yield return null;
		sounds = GameObject.FindObjectsOfType<UIPlaySound>();
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
		foreach(UIPlaySound sound in sounds)
			sound.enabled = !mute;
	}
	
	public static bool Muted
	{
		get{
			if(_instance == null) return false;
			return _instance.mute;
		}
	}
}

