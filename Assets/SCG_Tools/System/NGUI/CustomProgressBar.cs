using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent (typeof(UIProgressBar))]
public class CustomProgressBar : MonoBehaviour
{
	[SerializeField]
	private float numerator;
	public float Numerator
	{
		get{ return numerator; }
		set{
			numerator = Mathf.Clamp(value, 0, denominator);
			int i = 0;
			while(numerator > ranges[i].highRange)
				i++;
			float percentInRange = (numerator - ranges[i].lowRange) / ranges[i].rangeDiff;
			_pBar.value = (percentInRange * ranges[i].percentDiff) + ranges[i].lowPercent;
		}
	}
	public float denominator;
	[SerializeField]
	private List<PercentRange> ranges = new List<PercentRange>();
	private UIProgressBar _pBar;
	public List<EventDelegate> CurrentRangeEvents
	{
		get{ 
			int i = 0;
			while(numerator > ranges[i].highRange)
				i++;
			return ranges[i].rangeEvents;
		}
	}

	void OnEnable()
	{
		if(_pBar == null)
			_pBar = GetComponent<UIProgressBar>();
	}

#if UNITY_EDITOR
	void Update()
	{
		if(_pBar != null && ranges != null && ranges.Count > 0)
			Numerator = numerator;
	}
#endif


	[System.Serializable]
	private class PercentRange
	{
		[Range(0, 1)]
		public float lowPercent = 0;
		[Range(0, 1)]
		public float highPercent = 1;
		public float lowRange = 0;
		public float highRange = 1;
		public List<EventDelegate> rangeEvents = new List<EventDelegate>();

		public float rangeDiff
		{
			get{ return highRange - lowRange; }
		}

		public float percentDiff
		{
			get{ return highPercent - lowPercent; }
		}
	}
}

