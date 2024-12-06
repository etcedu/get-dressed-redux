using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween To Transform")]
public class TweenToTransform : UITweener
{
	public Transform to;
	public bool parentWhenFinished = false;
	
	private Transform from;
	
	
	Transform mTrans;
	Vector3 mPos;
	Quaternion mRot;
	Vector3 mScale;
	
	/// <summary>
	/// Interpolate the position, scale, and rotation.
	/// </summary>
	
	protected override void OnUpdate (float factor, bool isFinished)
	{
		if (to != null)
		{
			if (mTrans == null)
			{
				mTrans = transform;
				mPos = mTrans.position;
				mRot = mTrans.rotation;
				mScale = mTrans.localScale;
			}

			if(from == null) {
				GameObject f = new GameObject("from");
				f.transform.position = transform.position;
				f.transform.rotation = transform.rotation;
				f.transform.localScale = transform.localScale;
				from = f.transform;
			}

			mTrans.position = from.position * (1f - factor) + to.position * factor;
			mTrans.localScale = from.localScale * (1f - factor) + to.localScale * factor;
			mTrans.rotation = Quaternion.Slerp(from.rotation, to.rotation, factor);
			
			if(isFinished) {
				Destroy(from.gameObject);
				// Change the parent when finished, if requested
				if (parentWhenFinished) {
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
	
	static public TweenToTransform Begin (GameObject go, float duration, Transform to)
	{
		TweenToTransform comp = UITweener.Begin<TweenToTransform>(go, duration);
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
}