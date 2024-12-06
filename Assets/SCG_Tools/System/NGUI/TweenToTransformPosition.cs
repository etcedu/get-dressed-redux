using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween To Transform Position")]
public class TweenToTransformPosition : UITweener
{
    public Transform to;
    public bool parentWhenFinished = false;

    private Transform from;


    Transform mTrans;
    Vector3 mPos;

    /// <summary>
    /// Interpolate the position, scale, and rotation.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        if (to != null)
        {
            if (mTrans == null)
            {
                mTrans = transform;
                mPos = mTrans.position;
            }

            if (from == null)
            {
                GameObject f = new GameObject("from");
                f.transform.position = transform.position;
                from = f.transform;
            }

            mTrans.position = from.position * (1f - factor) + to.position * factor;

            if (isFinished)
            {
                Destroy(from.gameObject);
                // Change the parent when finished, if requested
                if (parentWhenFinished)
                {
                    mTrans.parent = to;
                }
            }
        }
    }

    void OnDisable()
    {
        if (from != null)
            Destroy(from.gameObject);
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenToTransformPosition Begin(GameObject go, float duration, Transform to)
    {
        TweenToTransformPosition comp = UITweener.Begin<TweenToTransformPosition>(go, duration);

        comp.from = null;
        comp.to = to;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }
}