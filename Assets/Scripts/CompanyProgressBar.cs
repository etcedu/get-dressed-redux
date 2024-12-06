using UnityEngine;
using System.Collections.Generic;

[RequireComponent (typeof(UIProgressBar))]
public class CompanyProgressBar : MonoBehaviour
{
	[Range(0, 4)]
	public int rank;

	private float _numerator;
	private float Numerator
	{
		get{ return _numerator; }
		set{
			if(value == _numerator || _pBar == null) return;
			if(Denominator == 0)
				_pBar.value = 0;
			else
			{
				_numerator = Mathf.Clamp(value, 0, Denominator);
				_pBar.value = _numerator / Denominator;
			}
		}
	}

	private float Denominator
	{
		get{
			return CrossSceneInfo.GetRankCutoff(rank);
		}
	}
	private UIProgressBar _pBar;
	
	void Start()
	{
		gameObject.SetActive(rank != 0 && CrossSceneInfo.TotalScore < CrossSceneInfo.GetRankCutoff(rank));
		_pBar = GetComponent<UIProgressBar>();
		Numerator = CrossSceneInfo.TotalScore;
	}
}

