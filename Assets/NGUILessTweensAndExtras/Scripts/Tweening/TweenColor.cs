//-------------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2017 Tasharen Entertainment Inc
//-------------------------------------------------
//
//   Edited for use with UUI and non-NGUI games
//               Garrett Kimball
//             Simcoach Games 2016
//
//-------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tween the object's color.
/// </summary>

[AddComponentMenu("NGUILessTweens/Tween Color")]
public class TweenColor : UITweener
{
	public Color from = Color.white;
	public Color to = Color.white;

	bool mCached = false;
	Material mMat;
	Light mLight;
	SpriteRenderer mSr;
    Image mImg;
    RawImage mRaw;
    Text mText;

    void Cache ()
	{
		mCached = true;

		mSr = GetComponent<SpriteRenderer>();
        mImg = GetComponent<Image>();
        mText = GetComponent<Text>();
        mRaw = GetComponent<RawImage>();

        if (mSr == null)
        {
            Renderer ren = GetComponent<Renderer>();
            if (ren != null) mMat = ren.sharedMaterial;
        }

        mLight = GetComponent<Light>();
	}

	[System.Obsolete("Use 'value' instead")]
	public Color color { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public Color value
	{
		get
		{
			if (!mCached) Cache();
			if (mMat != null) return mMat.color;
			if (mSr != null) return mSr.color;
            if (mImg != null) return mImg.color;
            if (mText != null) return mText.color;
            if (mRaw != null) return mRaw.color;
            if (mLight != null) return mLight.color;
			return Color.black;
		}
		set
		{
			if (!mCached) Cache();
			else if (mMat != null) mMat.color = value;
			else if (mSr != null) mSr.color = value;
            else if(mImg != null) mImg.color = value;
            else if(mText != null) mText.color = value;
            else  if (mRaw != null) mRaw.color = value;
            else if (mLight != null)
			{
				mLight.color = value;
				mLight.enabled = (value.r + value.g + value.b) > 0.01f;
			}
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenColor Begin (GameObject go, float duration, Color color)
	{
#if UNITY_EDITOR
		if (!Application.isPlaying) return null;
#endif
		TweenColor comp = UITweener.Begin<TweenColor>(go, duration);
		comp.from = comp.value;
		comp.to = color;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	[ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
	void SetCurrentValueToEnd () { value = to; }
}
