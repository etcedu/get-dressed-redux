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
/// Tween the sprite's fill.
/// </summary>

[AddComponentMenu("NGUILessTweens/Tween Fill")]
public class TweenFill : UITweener
{
    [Range(0f, 1f)]
    public float from = 1f;
    [Range(0f, 1f)]
    public float to = 1f;

    bool mCached = false;
    Image mSprite;

    void Cache()
    {
        mCached = true;
        mSprite = GetComponent<Image>();
    }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public float value
    {
        get
        {
            if (!mCached) Cache();
            if (mSprite != null) return mSprite.fillAmount;
            return 0f;
        }
        set
        {
            if (!mCached) Cache();
            if (mSprite != null) mSprite.fillAmount = value;
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenFill Begin(GameObject go, float duration, float fill)
    {
        TweenFill comp = UITweener.Begin<TweenFill>(go, duration);
        comp.from = comp.value;
        comp.to = fill;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    public override void SetStartToCurrentValue() { from = value; }
    public override void SetEndToCurrentValue() { to = value; }
}
