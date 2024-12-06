using UnityEngine;
using System.Collections.Generic;

public class BoolSet : MonoBehaviour
{
	[SerializeField]
	public string boolKey = "";
	[SerializeField]
	private bool setTo = true;
	[SerializeField]
	private bool setOnStart = false;
	
	public void Execute()
	{
		Set();
	}
	
	// Use this for initialization
	void Start ()
	{
		if(setOnStart)
			Set();
	}
	
	void Set()
	{
		GameBase.Bools.SetValue(boolKey, setTo);
		GameBase.Bools.Save();
	}

	public void Toggle()
	{
		bool current = GameBase.Bools.GetValue(boolKey, false);
		GameBase.Bools.SetValue(boolKey, !current);
		GameBase.Bools.Save();
	}
}

