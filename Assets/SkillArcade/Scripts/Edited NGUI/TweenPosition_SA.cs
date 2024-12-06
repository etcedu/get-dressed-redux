//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2016 Tasharen Entertainment
//----------------------------------------------

/*-------------------*
 * Edited for Skill Arcade portable use by Garrett Kimball  
 * ------------------*/
using UnityEngine;

/// <summary>
/// Tween the object's position.
/// </summary>

namespace Simcoach.SkillArcade
{
    public class TweenPosition_SA : UITweener_SA
    {
        public Vector3 from;
        public Vector3 to;
        public bool ignoreEqualFields;

        [HideInInspector]
        public bool worldSpace = false;

        bool mCached = false;
        Transform mTrans;
        RectTransform mRect;

        void Cache()
        {
            mCached = true;
            mTrans = GetComponent<Transform>();
            mRect = GetComponent<RectTransform>();
        }

        [System.Obsolete("Use 'value' instead")]
        public Vector3 position { get { return this.value; } set { this.value = value; } }

        /// <summary>
        /// Tween's current value.
        /// </summary>

        public Vector3 value
        {
            get
            {
                if (!mCached) Cache();

                if (worldSpace)
                {
                    if (mRect != null) return mRect.position;
                    if (mTrans != null) return mTrans.position;
                }
                else
                {
                    if (mRect != null) return mRect.anchoredPosition;
                    if (mTrans != null) return mTrans.localPosition;
                }
                return mTrans.position;
            }
            set
            {
                if (!mCached) Cache();

                if (ignoreEqualFields)
                {
                    if (from.x == to.x)
                    {
                        if (mRect == null)
                            value.x = worldSpace ? mTrans.position.x : mTrans.localPosition.x;
                        else
                            value.x = worldSpace ? mRect.position.x : mRect.anchoredPosition.x;
                    }

                    if (from.y == to.y)
                    {
                        if (mRect == null)
                            value.y = worldSpace ? mTrans.position.y : mTrans.localPosition.y;
                        else
                            value.y = worldSpace ? mRect.position.y : mRect.anchoredPosition.y;
                    }

                    if (from.z == to.z)
                    {
                        if (mRect == null)
                            value.z = worldSpace ? mTrans.position.z : mTrans.localPosition.z;
                    }
                }

                if (mRect == null)
                {
                    if (worldSpace) mTrans.position = value;
                    else mTrans.localPosition = value;
                }
                else
                {
                    if (worldSpace) mRect.position = value;
                    else mRect.anchoredPosition = value;
                }
            }
        }

        void Awake() { mRect = GetComponent<RectTransform>(); }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenPosition_SA Begin(GameObject go, float duration, Vector3 pos)
        {
            TweenPosition_SA comp = UITweener_SA.Begin<TweenPosition_SA>(go, duration);
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        /// <summary>
        /// Start the tweening operation.
        /// </summary>

        static public TweenPosition_SA Begin(GameObject go, float duration, Vector3 pos, bool worldSpace)
        {
            TweenPosition_SA comp = UITweener_SA.Begin<TweenPosition_SA>(go, duration);
            comp.worldSpace = worldSpace;
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue() { from = value; }

        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue() { to = value; }

        [ContextMenu("Assume value of 'From'")]
        void SetCurrentValueToStart() { value = from; }

        [ContextMenu("Assume value of 'To'")]
        void SetCurrentValueToEnd() { value = to; }
    }
}
