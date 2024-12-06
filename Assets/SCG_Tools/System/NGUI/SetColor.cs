using UnityEngine;
using System.Collections;

public class SetColor : MonoBehaviour
{
	[SerializeField]
	private UIWidget[] targets = new UIWidget[0];
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
		foreach(UIWidget target in targets)
			target.color = color;
	}
}

