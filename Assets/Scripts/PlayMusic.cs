using UnityEngine;
using System.Collections;

public class PlayMusic : MonoBehaviour
{
	public AudioClip clip;

	public void Execute()
	{
		Music.PlaySong(clip);
	}
}

