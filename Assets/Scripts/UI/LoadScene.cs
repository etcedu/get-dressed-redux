using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour
{
	public string Scene;

	public void OnClick()
	{
		if (string.IsNullOrEmpty(Scene)) return;

		Application.LoadLevel(Scene);
	}
}

