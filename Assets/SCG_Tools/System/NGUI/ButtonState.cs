using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ButtonState {
	public string state = "";
	public GameObject button;
	public string spriteNormal = "", spriteHover = "",
	spritePressed = "", spriteDisabled = "";
	public Sprite sprite2D_Normal, sprite2D_Hover,
	sprite2D_Pressed, sprite2D_Disabled;
	public Color colorNormal = Color.white, colorHover = Color.gray,
	colorPressed = Color.grey.MultRGB(.5f), colorDisabled = Color.grey.MultA(.5f);
	public List<EventDelegate> onClickEvents = new List<EventDelegate>();
	private Dictionary<int, EventDelegate> _removeEvents = new Dictionary<int, EventDelegate>();
	
	public void Activate(bool codeClickExecute) {
		if(button == null) return;
		UIButton _button = button.GetComponent<UIButton>();
		if(_button == null) return;
		_button.normalSprite = spriteNormal;
		_button.normalSprite2D = sprite2D_Normal;
		_button.defaultColor = colorNormal;
		_button.hoverSprite = spriteHover;
		_button.normalSprite2D = sprite2D_Hover;
		_button.hover = colorHover;
		_button.pressedSprite = spritePressed;
		_button.pressedSprite2D = sprite2D_Pressed;
		_button.pressed = colorPressed;
		_button.disabledSprite = spriteDisabled;
		_button.disabledSprite2D = sprite2D_Disabled;
		_button.disabledColor = colorDisabled;
		if(!codeClickExecute)
			_button.onClick = onClickEvents;
	}
	
	public void CodeClick() {
		EventDelegate.Execute(onClickEvents);
	}
	public void CodeClick(List<EventDelegate> extraEvents)
	{
		int random = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
		EventDelegate remove = new EventDelegate((()=>RemoveEvents(extraEvents, random)));
		_removeEvents.Add(random, remove);
		onClickEvents.AddRange(extraEvents);
		onClickEvents.Add(remove);
		EventDelegate.Execute(onClickEvents);
	}
	void RemoveEvents(List<EventDelegate> eventsToRemove, int removeEvent)
	{
		foreach(EventDelegate ed in eventsToRemove)
			onClickEvents.Remove(ed);
		onClickEvents.Remove(_removeEvents[removeEvent]);
		_removeEvents.Remove(removeEvent);
	}
	
	public void SetTo(UIButton btn) {
		this.button = btn.gameObject;
		this.spriteNormal = btn.normalSprite;
		this.colorNormal = btn.defaultColor;
		this.spriteHover = btn.hoverSprite;
		this.colorHover = btn.hover;
		this.spritePressed = btn.pressedSprite;
		this.colorPressed = btn.pressed;
		this.spriteDisabled = btn.disabledSprite;
		this.colorDisabled = btn.disabledColor;
		this.onClickEvents = new List<EventDelegate>(btn.onClick);
	}
}