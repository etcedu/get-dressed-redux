using UnityEngine;
using System.Collections.Generic;

public class ExamineToggle : MonoBehaviour
{
	[SerializeField]
	private bool enabledOnStart;
	[SerializeField]
	private bool hideOnClick;
	[SerializeField]
	private bool hideOnRepeat;
	[SerializeField]
	private List<GameObject> show;
	[SerializeField]
	private List<GameObject> hide;
	private GameObject _lastShow;

	
	void Awake()
	{
		foreach(GameObject go in show)
			go.AddMissingComponent<ForwardTouch>().DoubleClicked += ShowClicked;
		foreach(GameObject go in hide)
			go.AddMissingComponent<ForwardTouch>().DoubleClicked += HideClicked;
		gameObject.SetActive(enabledOnStart);
	}


	void ShowClicked(GameObject target)
	{
		if(!hideOnRepeat || _lastShow != target)
		{
			gameObject.SetActive(true);
			_lastShow = target;
		} else
		{
			gameObject.SetActive(false);
			_lastShow = null;
		}
	}


	void HideClicked(GameObject target)
	{
		gameObject.SetActive(false);
	}


	void OnClick() 
	{
		if(hideOnClick)
		{
			_lastShow = null;
			gameObject.SetActive(false);
		}
	}
}

