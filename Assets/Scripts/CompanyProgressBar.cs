using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
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
				_pBar.fillAmount = 0;
			else
			{
				_numerator = Mathf.Clamp(value, 0, Denominator);
				_pBar.fillAmount = _numerator / Denominator;
			}
		}
	}

	private float Denominator
	{
		get{
			return CrossSceneInfo.GetRankCutoff(rank);
		}
	}
	[SerializeField] private Image _pBar;
	
	void Start()
	{
		gameObject.SetActive(rank != 0 && CrossSceneInfo.TotalScore < CrossSceneInfo.GetRankCutoff(rank));
		_pBar = transform.GetChild(0).GetComponent<Image>();
		Numerator = CrossSceneInfo.TotalScore;
	}
}

