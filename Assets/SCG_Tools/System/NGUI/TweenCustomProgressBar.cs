using UnityEngine;
using System.Collections;

[AddComponentMenu("NGUI/Tween/Tween Custom Progress Bar")]
public class TweenCustomProgressBar : UITweener
{
	public CustomProgressBar target;
	public float numeratorFrom, numeratorTo;
	public bool inheritNumeratorAsFrom;

	private float? _from, _to;
	
	/// <summary>
	/// Interpolate the numerator.
	/// </summary>
	
	protected override void OnUpdate(float factor, bool isFinished)
	{
		if (_from == null)
		{
			if(inheritNumeratorAsFrom)
				_from = target.Numerator;
			else
				_from = numeratorFrom;
			_to = numeratorTo;
		}

		target.Numerator = _from.Value * (1f - factor) + _to.Value * factor;
		
		if (isFinished)
		{
			target.Numerator = (float)_to;
			EventDelegate.Execute(target.CurrentRangeEvents);
		}
	}
	
	void OnDisable()
	{
		if (_from != null)
		{
			_from = null;
			_to = null;
		}
	}
	
	/// <summary>
	/// Start the tweening operation.
	/// </summary>
	
	static public TweenCustomProgressBar Begin(GameObject go, float duration)
	{
		TweenCustomProgressBar comp = UITweener.Begin<TweenCustomProgressBar>(go, duration);

		comp._from = null;
		comp._to = null;
		
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}
}

