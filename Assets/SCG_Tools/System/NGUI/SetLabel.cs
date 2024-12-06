using UnityEngine;
using System.Collections;

public class SetLabel : MonoBehaviour {
	[SerializeField]
	protected string text;
	[SerializeField]
	protected UILabel label;
	[SerializeField]
	protected bool setOnStart;

	// Use this for initialization
	void Start () {
		if(setOnStart)
			Execute();
	}
	
	// Update is called once per frame
	public void Execute () {
		if(label == null)
		{
			label = GetComponent<UILabel>();
			if(label == null) return;
		}
		label.text = text;
	}
}
