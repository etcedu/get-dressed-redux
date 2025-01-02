using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

[RequireComponent (typeof(AudioSource))]
public class Music : MonoBehaviour {
	public string muteCheck = "MuteMusic";
	public AudioClip song;
	AudioSource source;
	static Music _instance;

	// Use this for initialization
	IEnumerator Start ()
	{
		yield return null;
	
		source = GetComponent<AudioSource>();
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
			source.mute = GameBase.Bools.GetValue(muteCheck, false);
			Music.PlaySong(song);

            SceneManager.sceneLoaded += LevelLoaded;
        } 
		else if(_instance != this)
		{
			Destroy(gameObject);
		}
	}

    private void LevelLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine(levelLoadedRoutine());
    }
		
	IEnumerator levelLoadedRoutine()
	{
		yield return null;
		if(_instance == this)
		{
			yield return null;
			Music[] songs = GameObject.FindObjectsOfType<Music>();
			
			foreach(Music m in songs)
			{
				if(m != this && m.song != source.clip)
				{
					Music.PlaySong(m.song);
					break;
				}
			}
		} else 
			yield return null;
	}

	void Update ()
	{
		if(source != null && GameBase.Bools.GetValue(muteCheck, false) != source.mute)
		{
			source.mute = !source.mute;
		}
	}

	public static void PlaySong(AudioClip clip)
	{
		if(_instance == null)
		{
			Debug.LogError("No Music instance!");
			return;
		}
		if(_instance.source == null)
		{
			Debug.LogError("Music instance's AudioSource is null!");
			return;
		}
		_instance.source.clip = clip;
		_instance.source.Play();
	}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= LevelLoaded;
    }
}
