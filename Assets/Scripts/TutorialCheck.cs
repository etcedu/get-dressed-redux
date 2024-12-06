using UnityEngine;
using System.Collections.Generic;

public class TutorialCheck : MonoBehaviour
{
	public string checkKey;
	public bool checkOnStart;
	public List<EventDelegate> firstTimeEvents = new List<EventDelegate>();

	// Use this for initialization
	void Start ()
	{
		if(checkOnStart)
			Check();
	}

	public void Check_ED()
	{
		Check();
	}

	public bool Check()
	{
		if(!GameBase.Bools.ContainsKey(checkKey) || !GameBase.Bools.GetValue(checkKey, false))
		{
			GameBase.Bools.SetValue(checkKey, true);
			GameBase.Bools.Save();
			EventDelegate.Execute(firstTimeEvents);
			return false;
		}
		return true;
	}
}

