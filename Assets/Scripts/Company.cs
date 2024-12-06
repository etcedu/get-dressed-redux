using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Company : MonoBehaviour {
	[SerializeField]
	string companyName;
	public string CompanyName
	{
		get{ return companyName; }
		set{ companyName = value; }
	}
	[SerializeField]
	CompanyPositionInfoButton positionOne = new CompanyPositionInfoButton(),
	positionTwo = new CompanyPositionInfoButton(),
	positionThree = new CompanyPositionInfoButton();
	public CompanyPositionInfo PositionOne
	{
		get{ return positionOne.info; }
		set{ positionOne.info = value; }
	}
	public UIButton PositionOneButton
	{
		get{ return positionOne.button; }
		set{ positionOne.button = value; }
	}
	public CompanyPositionInfo PositionTwo
	{
		get{ return positionTwo.info; }
		set{ positionTwo.info = value; }
	}
	public UIButton PositionTwoButton
	{
		get{ return positionTwo.button; }
		set{ positionTwo.button = value; }
	}
	public CompanyPositionInfo PositionThree
	{
		get{ return positionThree.info; }
		set{ positionThree.info = value; }
	}
	public UIButton PositionThreeButton
	{
		get{ return positionThree.button; }
		set{ positionThree.button = value; }
	}

#if UNITY_EDITOR
	void Update()
	{
		if(Application.isPlaying)
			return;
		if(positionOne.button != null)
			positionOne.button.GetComponentInChildren<UILabel>().text = positionOne.info.PositionName;
		if(positionTwo.button != null)
			positionTwo.button.GetComponentInChildren<UILabel>().text = positionTwo.info.PositionName;
		if(positionThree.button != null)
			positionThree.button.GetComponentInChildren<UILabel>().text = positionThree.info.PositionName;
	}
#endif

	[System.Serializable]
	class CompanyPositionInfoButton
	{
		[SerializeField]
		public UIButton button;
		[SerializeField]
		public CompanyPositionInfo info = new CompanyPositionInfo();
	}
}
