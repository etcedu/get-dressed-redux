using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class RankLabel : MonoBehaviour
{
	void Start()
	{
		GetComponent<UILabel>().text = CrossSceneInfo.RankName;
	}
}
