//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2015 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's position, rotation and scale.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Sprite Fill")]
[RequireComponent (typeof(UISprite))]
public class TweenSpriteFill : UITweener
{
	public float from;
	public float to;
	
	UISprite mSprite;
	
	/// <summary>
	/// Interpolate the position, scale, and rotation.
	/// </summary>
	
	protected override void OnUpdate (float factor, bool isFinished)
	{
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
		}

		mSprite.fillAmount = from * (1f - factor) + to * factor;
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenSpriteFill Begin (GameObject go, float duration, float from, float to)
	{
		TweenSpriteFill comp = UITweener.Begin<TweenSpriteFill>(go, duration);
		comp.from = from;
		comp.to = to;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}
