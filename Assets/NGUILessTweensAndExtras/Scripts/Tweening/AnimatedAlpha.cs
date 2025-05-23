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
/// Makes it possible to animate alpha of the widget or a panel.
/// </summary>

[ExecuteInEditMode]
public class AnimatedAlpha : MonoBehaviour
{
	[Range(0f, 1f)]
	public float alpha = 1f;

    Material mMat;
    SpriteRenderer mSr;
    Image mImg;
    RawImage mRaw;
    Text mText;
    CanvasGroup mCanvas;

    void OnEnable ()
	{
        mSr = GetComponent<SpriteRenderer>();
        mImg = GetComponent<Image>();
        mText = GetComponent<Text>();
        mRaw = GetComponent<RawImage>();
        mCanvas = GetComponent<CanvasGroup>();

        if (mSr == null)
        {
            Renderer ren = GetComponent<Renderer>();
            if (ren != null) mMat = ren.material;
        }

        LateUpdate();
	}

	void LateUpdate ()
	{
        if (mCanvas != null)
        {
            mCanvas.alpha = alpha;
        }
        else if (mSr != null)
        {
            Color c = mSr.color;
            c.a = alpha;
            mSr.color = c;
        }
        else if (mMat != null)
        {
            Color c = mMat.color;
            c.a = alpha;
            mMat.color = c;
        }
        else if (mImg != null)
        {
            Color c = mImg.color;
            c.a = alpha;
            mImg.color = c;
        }
        else if (mRaw != null)
        {
            Color c = mRaw.color;
            c.a = alpha;
            mRaw.color = c;
        }
        else if (mText != null)
        {
            Color c = mText.color;
            c.a = alpha;
            mText.color = c;
        }
    }
}
