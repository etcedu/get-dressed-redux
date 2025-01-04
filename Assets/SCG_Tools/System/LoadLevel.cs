using UnityEngine;
using System.Collections.Generic;

public class LoadLevel : MonoBehaviour
{
	public string levelName;

	public void Load()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
	}

	public void ResetLevel()
	{
		EventRecorder.RecordLevelRestarted();
		Load();
	}
}

