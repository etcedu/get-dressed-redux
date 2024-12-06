using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class LockAndKey : MonoBehaviour
{
	public int unlockRank;
	public List<string> unlockPassedLevels;
	[SerializeField]
	GameObject target;
	[SerializeField]
	HideOrDisable hideOrDisable;
	[SerializeField]
	List<EventDelegate> hideOrDisableEvents = new List<EventDelegate>();
	[SerializeField]
	List<EventDelegate> unlockedEvents = new List<EventDelegate>();

	// Use this for initialization
	void Start()
	{
		if(CrossSceneInfo.Rank < unlockRank)
		{
			performHideOrDisable();
			return;
		}

		foreach(string key in unlockPassedLevels)
		{
			if(!CrossSceneInfo.PassedInterviews.Contains(key))
			{
				performHideOrDisable();
				return;
			}
		}

		EventDelegate.Execute(unlockedEvents);
	}

	void performHideOrDisable()
	{
		if(target == null)
			target = gameObject;
		UIButton button = target.GetComponent<UIButton>();
		if(button != null && hideOrDisable == HideOrDisable.DISABLE)
			button.isEnabled = false;
		else
			target.SetActive(false);
		EventDelegate.Execute(hideOrDisableEvents);
	}

	enum HideOrDisable
	{
		HIDE, DISABLE
	}
}

