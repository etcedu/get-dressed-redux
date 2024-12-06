using UnityEngine;
using System.Collections.Generic;

public class LoadLevel : MonoBehaviour {
	public string levelName;

	public void Load() {
		Application.LoadLevel(levelName);
	}

	public void ResetLevel()
	{
		EventRecorder.RecordLevelRestarted();
		Load();
	}
}

