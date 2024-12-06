//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

/*-------------------*
 * Edited for Skill Arcade portable use by Garrett Kimball  
 * ------------------*/

using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Tween the object's alpha. Works with both UI widgets as well as renderers.
/// </summary>

namespace Simcoach.SkillArcade
{

    [AddComponentMenu("NGUI/Tween/Tween Alpha SA")]
    public class TweenAlpha_SA : UITweener_SA
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;

        bool mCached = false;
        //UIRect mRect;
        Material mMat;
        SpriteRenderer mSr;
        Image mImg;
        RawImage mRaw;
        Text mText;
        CanvasGroup mCanvas;

        [System.Obsolete("Use 'value' instead")]
        public float alpha { get { return this.value; } set { this.value = value; } }

        void Cache()
        {
            mCached = true;
            //mRect = GetComponent<UIRect>();
            mSr = GetComponent<SpriteRenderer>();
            mImg = GetComponent<Image>();
            mText = GetComponent<Text>();
            mRaw = GetComponent<RawImage>();
            mCanvas = GetComponent<CanvasGroup>();

            if (mSr == null)
            {
                Renderer ren = GetComponent<Renderer>();
                if (ren != null) mMat = ren.material;
                //if (mMat == null) mRect = GetComponentInChildren<UIRect>();
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
                //if (mRect != null) return mRect.alpha;
                if (mCanvas != null) return mCanvas.alpha;
                if (mSr != null) return mSr.color.a;
                if (mImg != null) return mImg.color.a;
                if (mText != null) return mText.color.a;
                if (mRaw != null) return mRaw.color.a;
                return mMat != null ? mMat.color.a : 1f;
            }
            set
            {
                if (!mCached) Cache();

                //if (mRect != null)
                //{
                // mRect.alpha = value;
                //}
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
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenAlpha_SA Begin(GameObject go, float duration, float alpha)
        {
            TweenAlpha_SA comp = UITweener_SA.Begin<TweenAlpha_SA>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

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

}