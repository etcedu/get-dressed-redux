using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

public class MapManager : MonoBehaviour
{
	[SerializeField]
	TextAsset data;
	[SerializeField]
	Company[] companies = new Company[0];
	[SerializeField]
	string dressSceneName = "";
	[SerializeField]
	List<UIButton> categories = new List<UIButton>();

	// Ties the game object's click action with the position info
	Dictionary<GameObject, CompanyPositionInfo> togglePositionDictionary = new Dictionary<GameObject, CompanyPositionInfo>();
	// Used when closing a company without unselecting a selected position
	Dictionary<CompanyPositionInfo, ButtonStateToggle> companyCloseDictionary = new Dictionary<CompanyPositionInfo, ButtonStateToggle>();
	// Used when opening a company without having closed a previously opened one
	Dictionary<GameObject, GameObject> toggleCompanyDictionay = new Dictionary<GameObject, GameObject>();
	[System.NonSerialized]
	CompanyPositionInfo _lastSelectedPosition = null;
	[System.NonSerialized]
	GameObject _lastSelectedCompany = null;
	HashSet<Company> unlockedCompanies = new HashSet<Company>();

	// Use this for initialization
	void Awake ()
	{
		foreach(Company pin in companies)
		{
			GameObject button = pin.gameObject.GetChildGameObject("Button");
			button.GetComponent<Button>().onClick.AddListener(() => CompanyClicked(button));
			//ButtonStateToggle bst = button.GetComponent<ButtonStateToggle>();;
			toggleCompanyDictionay.Add(button, pin.detailsPopup);

			GameObject p1 = pin.PositionOneButton.gameObject;
			GameObject p2 = pin.PositionTwoButton.gameObject;
			GameObject p3 = pin.PositionThreeButton.gameObject;

			togglePositionDictionary.Add(p1, pin.PositionOne);
			companyCloseDictionary.Add(pin.PositionOne, p1.GetComponent<ButtonStateToggle>());

			togglePositionDictionary.Add(p2, pin.PositionTwo);
			companyCloseDictionary.Add(pin.PositionTwo, p2.GetComponent<ButtonStateToggle>());

			togglePositionDictionary.Add(p3, pin.PositionThree);
			companyCloseDictionary.Add(pin.PositionThree, p3.GetComponent<ButtonStateToggle>());

			p1.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;
			p2.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;
			p3.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;

			pin.gameObject.GetChildGameObject("Position1_Info").GetChildGameObject("Apply")
				.GetComponent<Button>().onClick.AddListener(()=> ConfirmPosition());
			pin.gameObject.GetChildGameObject("Position2_Info").GetChildGameObject("Apply")
				.GetComponent<Button>().onClick.AddListener(()=> ConfirmPosition());
            pin.gameObject.GetChildGameObject("Position3_Info").GetChildGameObject("Apply")
				.GetComponent<Button>().onClick.AddListener(() => ConfirmPosition());

            if (pin.gameObject.activeSelf)
				unlockedCompanies.Add(pin);
		}
	}

	void Start()
	{
		UIButton selectedButton = categories[GameBase.Ints.GetValue("LastCategory", 1) - 1];
		if(selectedButton != null)
		{
			EventDelegate.Execute(selectedButton.onClick);
		}
	}

	void PositionClicked(GameObject key)
	{
		CompanyPositionInfo cpi = togglePositionDictionary[key];

		if(_lastSelectedPosition != null)
		{
			bool samePosition = cpi == _lastSelectedPosition;
			ClosePosition();
			if(samePosition)
				return;
		}
		_lastSelectedPosition = cpi;
	}

	void ConfirmPosition()
	{
		/*
		var dataEvent = new SkillEvent();
		dataEvent.AddString("EventType", "StartLevel");
		dataEvent.AddString("Company", _lastSelectedPosition.CompanyName);
		dataEvent.AddString("Position", _lastSelectedPosition.PositionName);
		dataEvent.Save();
		*/

		Guid guid = Guid.NewGuid();
		CrossSceneInfo.LevelAttemptId = guid;
		CrossSceneInfo.LevelStartedTimeStamp = DateTime.Now;
		CrossSceneInfo.LastCompanyName = _lastSelectedPosition.CompanyName;
		CrossSceneInfo.LastPositionName = _lastSelectedPosition.PositionName;
		
		EventRecorder.RecordLevelStartedEvent(guid, _lastSelectedPosition.CompanyName, _lastSelectedPosition.PositionName);

		CrossSceneInfo.SelectedCompany = _lastSelectedPosition;
		//Application.LoadLevel(dressSceneName);
        SceneManager.LoadScene(dressSceneName);
	}
	
	void ClosePosition()
	{
		if(_lastSelectedPosition == null)
			return;

		companyCloseDictionary[_lastSelectedPosition].SetToStart();
		_lastSelectedPosition = null;
	}

	void CompanyClicked(GameObject key)
	{
		Debug.Log("company clicked");
		if(_lastSelectedCompany == null)
		{
			_lastSelectedCompany = toggleCompanyDictionay[key];
			foreach(Company c in unlockedCompanies)
			{
				GameObject button = c.gameObject.GetChildGameObject("Button");
				if(button != key)
					c.gameObject.SetActive(false);
			}
		} else
			closeCompany();
	}
	
	public void CloseCompany()
	{
		if(_lastSelectedCompany != null)
		{
			_lastSelectedCompany.SetActive(false);
			closeCompany();
		}
	}

	void closeCompany()
	{
		if(_lastSelectedPosition != null)
			ClosePosition();
		
		foreach(Company c in unlockedCompanies)
			c.gameObject.SetActive(true);
		
		_lastSelectedCompany = null;
		_lastSelectedPosition = null;
	}
	
	#if UNITY_EDITOR
	public List<GameObject> pinPanels = new List<GameObject>();
	public GameObject basePinPanel;
	public GameObject basePin;
	[SerializeField]
	Canvas mapCanvas;

	[ContextMenu ("Import Companies")]
	public void ParseCompanies()//TextAsset data, bool clearItemsOnImport)
	{
		if(data == null)
			return;
		
		string[] items = data.text.Split(new char[]{'\n'});
		
		for(int i = 1; i < items.Length; i++)
		{
			string[] fields = items[i].Split(new char[]{'\t'});

			string companyName = fields[0];
			Company c = companies.FirstOrDefault(comp => comp.CompanyName == fields[0]);
			GameObject parent;
			int parentNum = int.Parse(fields[7]);
			if(pinPanels.Count < parentNum)
			{
				parent = Instantiate(basePinPanel, mapCanvas.transform) as GameObject;
				parent.name += "_"+parentNum;
			}
			else
				parent = pinPanels[parentNum - 1];

			if(c == null)
			{
				Company newCompany;
				GameObject pin = Instantiate(basePin) as GameObject;
				pin.transform.SetParent(parent.transform);
				pin.transform.localScale = Vector3.one;
				newCompany = pin.AddMissingComponent<Company>();
				newCompany.gameObject.name = companyName;
				newCompany.nameLabel.text = companyName;
                //pin.transform.Find("NameLabel").GetComponent<TMP_Text>().text = companyName;
				ArrayUtility.Add<Company>(ref companies, newCompany);
				c = companies.Last();
				c.CompanyName = companyName;
				c.PositionOne.CompanyName = companyName;
				c.PositionTwo.CompanyName = companyName;
				c.PositionThree.CompanyName = companyName;
			} else
				c.transform.SetParent(parent.transform);

			int listPosition = 0;
			if(!int.TryParse(fields[1], out listPosition))
				listPosition = 0;

			//GameObject positionContainer = c.transform.Find("Position Container").gameObject;
           
			CompanyPositionInfo cpi = null;
			if(listPosition == 0)
			{
				Debug.Log("setup " + c.CompanyName);
				c.mainButton.unlockRank = int.Parse(fields[4]);
				c.progressBar.rank = int.Parse(fields[4]);
				c.descLabel.text = fields[5];
				EditorUtility.SetDirty(c);
				continue;
			}
			else if(listPosition == 1)
			{
				cpi = c.PositionOne;
				c.PositionOneButton.GetComponent<LockAndKey>().unlockRank = int.Parse(fields[4]);
				c.star1.key = c.CompanyName + "_" + c.PositionOne.PositionName;
				EditorUtility.SetDirty(c);
				Debug.Log("setup " + c.CompanyName + ", position " + c.PositionOne.PositionName);
			}
			else if(listPosition == 2)
			{
				cpi = c.PositionTwo;
                c.PositionTwoButton.GetComponent<LockAndKey>().unlockRank = int.Parse(fields[4]);
                c.PositionTwoButton.GetComponent<LockAndKey>().unlockPassedLevels = new List<string>(){c.PositionOne.CompanyName + "_" + c.PositionOne.PositionName};
				c.star2.key = c.CompanyName + "_" + c.PositionTwo.PositionName;
				EditorUtility.SetDirty(c);
				Debug.Log("setup " + c.CompanyName + ", position " + c.PositionTwo.PositionName);
			}
			else
			{
				cpi = c.PositionThree;
                c.PositionThreeButton.GetComponent<LockAndKey>().unlockRank = int.Parse(fields[4]);
                c.PositionThreeButton.GetComponent<LockAndKey>().unlockPassedLevels = new List<string>() { c.PositionTwo.CompanyName + "_" + c.PositionTwo.PositionName };
                c.star3.key = c.CompanyName + "_" + c.PositionThree.PositionName;
				EditorUtility.SetDirty(c);
				Debug.Log("setup " + c.CompanyName + ", position " + c.PositionThree.PositionName);
			}
			
			string positionName = fields[2];
			string positionInfo = fields[6];
			cpi.PositionName = positionName;
			cpi.PositionInfo = positionInfo;

			c.pInfoContainerTitles[listPosition].text = positionName;
			c.pInfoContainerDescs[listPosition].text = positionInfo;

			string tier = fields[3];
			if(tier != "")
			{
				switch(tier)
				{
				case "Street":
					cpi.PositionDressTier = Clothing.Tier.TierEnum.INFORMAL;
					break;
				case "Casual":
					cpi.PositionDressTier = Clothing.Tier.TierEnum.CASUAL;
					break;
				case "Bus Casual":
					cpi.PositionDressTier = Clothing.Tier.TierEnum.BUSINESS_CASUAL;
					break;
				case "Bus Pro":
					cpi.PositionDressTier = Clothing.Tier.TierEnum.BUSINESS_PROFESSIONAL;
					break;
				}
			} else
				cpi.PositionDressTier = Clothing.Tier.TierEnum.INFORMAL;
		}
	}
#endif
}

