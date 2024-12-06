using UnityEngine;
using System.Collections;

[RequireComponent (typeof(UILabel))]
public class CurrentInterviewLabel : MonoBehaviour
{
	[SerializeField]
	private LabelType type;

	// Use this for initialization
	void Start ()
	{
        if (CrossSceneInfo.SelectedCompany == null)
            return;

		if(type == LabelType.NAME)
			GetComponent<UILabel>().text = CrossSceneInfo.SelectedCompany.PositionName;
		else
			GetComponent<UILabel>().text = CrossSceneInfo.SelectedCompany.PositionInfo;
	}

	private enum LabelType
	{
		NAME, INFO
	}
}

