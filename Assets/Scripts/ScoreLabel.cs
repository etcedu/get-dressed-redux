using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class ScoreLabel : MonoBehaviour
{
	void Start()
	{
		GetComponent<UILabel>().text = CrossSceneInfo.TotalScore.ToString();
	}
}
