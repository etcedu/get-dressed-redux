using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class CurrentRankCutoffLabel : MonoBehaviour
{
	void Start()
	{
		GetComponent<UILabel>().text = CrossSceneInfo.GetRankCutoff(CrossSceneInfo.Rank).ToString();
	}
}

