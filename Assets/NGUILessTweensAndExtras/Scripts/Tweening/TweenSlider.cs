//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween a slider
/// </summary>

[RequireComponent(typeof(Slider))]
[AddComponentMenu("NGUILessTweens/Tween Slider")]
public class TweenSlider : UITweener
{
	#if UNITY_3_5
	public float from = 1f;
	public float to = 1f;
	#else
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;
	#endif
	
	Slider mSource;
	
	/// <summary>
	/// Cached version of 'slider', as it's always faster to cache.
	/// </summary>
	
	public Slider sliderSource
	{
		get
		{
			if (mSource == null)
			{
				mSource = GetComponent<Slider>();
					
				if (mSource == null)
				{
					Debug.LogError("TweenSlider needs a Slider to work with", this);
					enabled = false;
				}
			}
			return mSource;
		}
	}
	
	/// <summary>
	/// Audio source's current volume.
	/// </summary>
	
	public float value
	{
		get
		{
			return sliderSource != null ? mSource.value : 0f;
		}
		set
		{
			if (sliderSource != null) mSource.value = value;
		}
	}
	
	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;
		mSource.enabled = (mSource.value > 0.01f);
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenSlider Begin (GameObject go, float duration, float targetValue)
	{
		TweenSlider comp = UITweener.Begin<TweenSlider>(go, duration);
		comp.from = comp.value;
		comp.to = targetValue;
		return comp;
	}
	
	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}
