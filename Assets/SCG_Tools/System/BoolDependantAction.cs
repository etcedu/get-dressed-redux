using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class BoolDependantAction : MonoBehaviour
{
	[SerializeField]
	public string boolKey = "";
	[SerializeField]
	private bool checkOnStart = false;
	[SerializeField]
	private bool executeOnCheck = false;
	[SerializeField] private bool recheckOnExecute = false;
	[SerializeField]
	private List<EventDelegate> trueEvents = new List<EventDelegate>();
	[SerializeField]
	private List<EventDelegate> falseEvents = new List<EventDelegate>();

    private bool _checked;
	private bool _boolValue;

	public List<EventDelegate> boolEvents
	{
		get{ 
			Check(true, recheckOnExecute);
			return (_boolValue ? trueEvents : falseEvents); 
		}
	}

	public void Execute()
	{
		EventDelegate.Execute(boolEvents);
	}

	// Use this for initialization
	void Start ()
	{
		if(checkOnStart)
			Check(false);
	}

	void Check(bool ignoreExecuteOnCheck, bool recheck = false)
	{
		if(!_checked || recheck)
		{
			_checked = true;
			_boolValue = GameBase.Bools.GetValue(boolKey, false);
		}
		if(executeOnCheck && !ignoreExecuteOnCheck)
			EventDelegate.Execute(_boolValue ? trueEvents : falseEvents);
	}
}

