using UnityEngine;
using System.Collections;
using TMPro;

public class RankLabel : MonoBehaviour
{
	void Start()
	{
		GetComponent<TMP_Text>().text = CrossSceneInfo.RankName;
	}
}
