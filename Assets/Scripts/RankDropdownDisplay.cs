using UnityEngine;
using System.Collections;

public class RankDropdownDisplay : MonoBehaviour
{
	[Range(0, 4)]
	public int rank;
	public Color currentRankColor;
	
	void Start()
	{
		UILabel label = GetComponent<UILabel>();
		label.text = CrossSceneInfo.GetRankName(rank);
		if(CrossSceneInfo.Rank == rank)
		{
			label.color = currentRankColor;
			gameObject.GetChildGameObject("highlight").SetActive(true);
		}
		else if(CrossSceneInfo.Rank < rank)
		{
			gameObject.GetChildGameObject("score").SetActive(true);
			gameObject.GetChild("score").GetComponent<UILabel>().text = CrossSceneInfo.GetRankCutoff(rank).ToString();
		}
	}
}

