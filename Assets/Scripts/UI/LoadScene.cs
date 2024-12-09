using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string Scene;

	public void Execute()
	{
		if (string.IsNullOrEmpty(Scene)) return;

		SceneManager.LoadScene(Scene);
	}
}

