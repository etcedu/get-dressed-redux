using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class RankCurrentDisplay : MonoBehaviour
{
	void Start()
	{
		GetComponent<UILabel>().text = CrossSceneInfo.RankName;
	}
}
