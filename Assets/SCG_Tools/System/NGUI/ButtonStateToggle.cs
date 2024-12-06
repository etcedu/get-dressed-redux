using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ButtonStateToggle : MonoBehaviour {
	public ButtonState _startingState = new ButtonState();
	public ButtonState _toggleState = new ButtonState();
	[SerializeField]
	private bool codeClickExecute = false;
	private bool _onStart = true;//, _setup = false;
	
	void OnEnable() {
		UIButton btn = GetComponent<UIButton>();
		if(btn != null) {
			if(_startingState.button == null) _startingState.SetTo(btn);
			if(_toggleState.button == null) _toggleState.SetTo(btn);
		}
	}
	void Awake() {
		if(Application.isPlaying) {
			if(!codeClickExecute)
			{
				EventDelegate execute = new EventDelegate(this, "Execute");
				_startingState.onClickEvents.Add(execute);
				_toggleState.onClickEvents.Add(execute);
			}
			_startingState.Activate(codeClickExecute);
		}
	}
	
	public void SetToStart() {
		_onStart = false;
		if(!codeClickExecute)
			_toggleState.CodeClick();
		else
			_toggleState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
	}
	
	public void SetToToggle() {
		_onStart = true;
		if(!codeClickExecute)
			_startingState.CodeClick();
		else
			_startingState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
	}

	public void Toggle()
	{
		_onStart = !_onStart;
		if(!codeClickExecute)
		{
			if(_onStart) _startingState.CodeClick();
			else _toggleState.CodeClick();
		}
		else
		{
			if(_onStart) _startingState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
			else _toggleState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
		}
	}

	public void EventDelegateToggle()
	{
		if(_onStart) _startingState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
		else _toggleState.CodeClick(new List<EventDelegate>(){new EventDelegate(this, "Execute")});
	}

	public bool IsToggled
	{
		get{ return !_onStart; }
	}
	
	void Execute() {
		_onStart = !_onStart;
		if(_onStart) {
			_startingState.Activate(codeClickExecute);
		}
		else {
			_toggleState.Activate(codeClickExecute);
		}
	}
	
	[ContextMenu ("Set Button To Start")]
	void SetButtonToStart() {
		_startingState.Activate(codeClickExecute);
	}
	[ContextMenu ("Set Button To Toggle")]
	void SetButtonToToggle() {
		_toggleState.Activate(codeClickExecute);
	}
}
