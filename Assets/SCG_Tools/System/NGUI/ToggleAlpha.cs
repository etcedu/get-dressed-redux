using UnityEngine;
using System.Collections;

public class ToggleAlpha : MonoBehaviour {
    [SerializeField]
    private float setTo = 1;
	[SerializeField]
	private bool setOnStart = false;
	bool visible = false;

    private UIWidget widget;
    private UIPanel panel;
    private UISprite sprite;

	void Awake() {
        widget = GetComponent<UIWidget>();
        if (widget != null)
        {
            visible = widget.alpha != 0;
            return;
        }

        panel = GetComponent<UIPanel>();
        if (panel != null)
        {
            visible = panel.alpha != 0;
            return;
        }

        sprite = GetComponent<UISprite>();
        if (sprite != null)
            visible = sprite.alpha != 0;
	}

	void Start()
	{
		if(setOnStart)
		{
			ToggleToValue();
		}
	}

	public void Toggle() {
		if(widget != null) {
			visible = widget.alpha != 0;
            widget.alpha = visible ? 1 : 0;
        }
        else if (panel != null)
        {
            visible = panel.alpha != 0;
            panel.alpha = visible ? 1 : 0;
        }
        else if (sprite != null)
        {
			visible = sprite.alpha != 0;
            sprite.alpha = visible ? 1 : 0;
		}
	}

	public void Enable() {
		visible = true;
		if(widget != null)
            widget.alpha = 1;
        else if (panel != null)
            panel.alpha = 1;
		else if(sprite != null)
			sprite.alpha = 1;
	}
	
	public void Disable() {
		visible = false;
		if(widget != null)
            widget.alpha = 0;
        else if (panel != null)
            panel.alpha = 0;
		else if(sprite != null)
			sprite.alpha = 0;
	}

    public void ToggleToValue()
    {
        if (widget != null)
        {
            visible = widget.alpha != 0;
            widget.alpha = visible ? setTo : 0;
        }
        else if (panel != null)
        {
            visible = panel.alpha != 0;
            panel.alpha = visible ? setTo : 0;
        }
        else if (sprite != null)
        {
            visible = sprite.alpha != 0;
            sprite.alpha = visible ? setTo : 0;
        }
    }

    public void EnableToValue()
    {
        visible = true;
        if (widget != null)
            widget.alpha = setTo;
        else if (panel != null)
            panel.alpha = setTo;
        else if (sprite != null)
            sprite.alpha = setTo;
    }
}
