using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetColor : MonoBehaviour
{
	[SerializeField]
	private Image[] targets = new Image[0];
	[SerializeField]
	private Color color = Color.white;
	[SerializeField]
	private bool setOnStart;

	void Start()
	{
		if(setOnStart)
			Set();
	}

	public void Set()
	{
		foreach(Image target in targets)
			target.color = color;
	}
}

