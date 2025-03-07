﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;


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
	public GameObject detailsPopup;

	[SerializeField]
	CompanyPositionInfoButton positionOne = new CompanyPositionInfoButton(),
	positionTwo = new CompanyPositionInfoButton(),
	positionThree = new CompanyPositionInfoButton();

	[SerializeField] public TMP_Text nameLabel;
	[SerializeField] public LockAndKey mainButton;
    [SerializeField] public GameObject positionContainer;
	[SerializeField] public CompanyProgressBar progressBar;
	[SerializeField] public TMP_Text descLabel;

	[SerializeField] public InterviewPassedDependantEvents star1, star2, star3;

	[SerializeField] public GameObject[] pInfoContainers;
	[SerializeField] public TMP_Text[] pInfoContainerTitles;
    [SerializeField] public TMP_Text[] pInfoContainerDescs;
    public CompanyPositionInfo PositionOne
	{
		get{ return positionOne.info; }
		set{ positionOne.info = value; }
	}
	public Button PositionOneButton
	{
		get{ return positionOne.button; }
		set{ positionOne.button = value; }
	}
	public CompanyPositionInfo PositionTwo
	{
		get{ return positionTwo.info; }
		set{ positionTwo.info = value; }
	}
	public Button PositionTwoButton
	{
		get{ return positionTwo.button; }
		set{ positionTwo.button = value; }
	}
	public CompanyPositionInfo PositionThree
	{
		get{ return positionThree.info; }
		set{ positionThree.info = value; }
	}
	public Button PositionThreeButton
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
			positionOne.button.GetComponentInChildren<TMP_Text>().text = positionOne.info.PositionName;
		if(positionTwo.button != null)
			positionTwo.button.GetComponentInChildren<TMP_Text>().text = positionTwo.info.PositionName;
		if(positionThree.button != null)
			positionThree.button.GetComponentInChildren<TMP_Text>().text = positionThree.info.PositionName;
	}
#endif

	[System.Serializable]
	class CompanyPositionInfoButton
	{
		[SerializeField] public Button button;
		[SerializeField] public CompanyPositionInfo info = new CompanyPositionInfo();
	}
}
