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
/// Tween the object's alpha. Works with both UI widgets as well as renderers.
/// </summary>

[AddComponentMenu("NGUILessTweens/Tween Alpha")]
public class TweenAlpha : UITweener
{
	[Range(0f, 1f)] public float from = 1f;
	[Range(0f, 1f)] public float to = 1f;

	bool mCached = false;
    SpriteRenderer mSr;
    Image mImg;
    RawImage mRaw;
    Text mText;
    CanvasGroup mCanvas;
    Material mMat;
	Light mLight;
	float mBaseIntensity = 1f;

	[System.Obsolete("Use 'value' instead")]
	public float alpha { get { return this.value; } set { this.value = value; } }

	void Cache ()
	{
		mCached = true;
        mSr = GetComponent<SpriteRenderer>();
        mImg = GetComponent<Image>();
        mText = GetComponent<Text>();
        mRaw = GetComponent<RawImage>();
        mCanvas = GetComponent<CanvasGroup>();

        if ( mSr == null)
		{
			mLight = GetComponent<Light>();

			if (mLight == null)
			{
				Renderer ren = GetComponent<Renderer>();
				if (ren != null) mMat = ren.material;
			}
			else mBaseIntensity = mLight.intensity;
		}
	}

	/// <summary>
	/// Tween's current value.
	/// </summary>

	public float value
	{
		get
		{
			if (!mCached) Cache();
			if (mSr != null) return mSr.color.a;
			return mMat != null ? mMat.color.a : 1f;
		}
		set
		{
			if (!mCached) Cache();

            if (mCanvas != null)
            {
                mCanvas.alpha = value;
            }
            else if (mSr != null)
			{
				Color c = mSr.color;
				c.a = value;
				mSr.color = c;
			}
			else if (mMat != null)
			{
				Color c = mMat.color;
				c.a = value;
				mMat.color = c;
			}
            else if (mImg != null)
            {
                Color c = mImg.color;
                c.a = value;
                mImg.color = c;
            }
            else if (mRaw != null)
            {
                Color c = mRaw.color;
                c.a = value;
                mRaw.color = c;
            }
            else if (mText != null)
            {
                Color c = mText.color;
                c.a = value;
                mText.color = c;
            }
            else if (mLight != null)
			{
				mLight.intensity = mBaseIntensity * value;
			}
		}
	}

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	static public TweenAlpha Begin (GameObject go, float duration, float alpha, float delay = 0f)
	{
		TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration, delay);
		comp.from = comp.value;
		comp.to = alpha;

		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

	public override void SetStartToCurrentValue () { from = value; }
	public override void SetEndToCurrentValue () { to = value; }
}
