using UnityEngine;
using System.Collections.Generic;

public class InterviewPassedDependantEvents : MonoBehaviour
{
	public string key;
	public List<EventDelegate> passedEvents = new List<EventDelegate>();
	public List<EventDelegate> notPassedEvents = new List<EventDelegate>();

	// Use this for initialization
	void Start ()
	{
		if(CrossSceneInfo.PassedInterviews.Contains(key))
			EventDelegate.Execute(passedEvents);
		else
			EventDelegate.Execute(notPassedEvents);
	}
}

