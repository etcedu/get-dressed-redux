using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween To Position")]
public class TweenToPosition : UITweener
{
    public Vector3 to;
    public bool isLocal;
    private Vector3? from;


    Transform mTrans;
    Vector3 mPos;
    Quaternion mRot;
    Vector3 mScale;

    /// <summary>
    /// Interpolate the position, scale, and rotation.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        if (mTrans == null)
        {
            mTrans = transform;
            mPos = mTrans.position;
            mRot = mTrans.rotation;
            mScale = mTrans.localScale;
        }

        if (from == null)
        {
        	if(isLocal)
        		from = transform.localPosition;
        	else
            	from = transform.position;
        }

        if (isLocal)
            mTrans.localPosition = (Vector3)from * (1f - factor) + to * factor;
        else
            mTrans.position = (Vector3)from * (1f - factor) + to * factor;

        if (isFinished)
        {
            from = null;
        }
    }

    void OnDisable()
    {
        if (from != null)
            from = null;
    }

    /// <summary>
    /// Start the tweening operation.
    /// </summary>

    static public TweenToPosition Begin(GameObject go, float duration, Vector3 to)
    {
        TweenToPosition comp = UITweener.Begin<TweenToPosition>(go, duration);
        /*GameObject f = new GameObject("from");
        f.transform.position = go.transform.position;
        f.transform.rotation = go.transform.rotation;
        f.transform.localScale = go.transform.localScale;*/
        comp.from = null;
        comp.to = to;

        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    public void SetTo(Vector3 to, bool isLocal)
    {
        this.to = to;
        this.isLocal = isLocal;
    }
    public void SetToAsCurrent(bool isLocal)
    {
        this.isLocal = isLocal;
        StartCoroutine(setToCurrent());
    }
    IEnumerator setToCurrent()
    {
        yield return null;
        yield return null;
        SetToCurrent();
    }
    [ContextMenu ("Set To to Current Value")]
    public void SetToCurrent()
    {
        if(isLocal)
            this.to = transform.localPosition;
        else
            this.to = transform.position;
    }
    [ContextMenu("Set Current to To")]
    public void SetCurrent()
    {
        if(isLocal)
            transform.localPosition = this.to;
        else
            transform.position = this.to;
    }
}