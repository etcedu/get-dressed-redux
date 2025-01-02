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
	List<Button> categories = new List<Button>();

	// Ties the game object's click action with the position info
	Dictionary<GameObject, CompanyPositionInfo> togglePositionDictionary = new Dictionary<GameObject, CompanyPositionInfo>();
	// Used when closing a company without unselecting a selected position
	Dictionary<CompanyPositionInfo, GameObject> companyCloseDictionary = new Dictionary<CompanyPositionInfo, GameObject>();
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
			pin.mainButton.GetComponent<Button>().onClick.AddListener(() => CompanyClicked(pin.mainButton.gameObject));
			toggleCompanyDictionay.Add(pin.mainButton.gameObject, pin.detailsPopup);
			pin.detailsPopup.SetActive(false);


			togglePositionDictionary.Add(pin.PositionOneButton.gameObject, pin.PositionOne);
			companyCloseDictionary.Add(pin.PositionOne, pin.pInfoContainers[0]);

			togglePositionDictionary.Add(pin.PositionTwoButton.gameObject, pin.PositionTwo);
			companyCloseDictionary.Add(pin.PositionTwo, pin.pInfoContainers[0]);

			togglePositionDictionary.Add(pin.PositionThreeButton.gameObject, pin.PositionThree);
			companyCloseDictionary.Add(pin.PositionThree, pin.pInfoContainers[0]);

			pin.PositionOneButton.onClick.AddListener(() => PositionClicked(pin.PositionOneButton.gameObject, pin, pin.pInfoContainers[0]));
            pin.PositionTwoButton.onClick.AddListener(() => PositionClicked(pin.PositionTwoButton.gameObject, pin, pin.pInfoContainers[1]));
            pin.PositionThreeButton.onClick.AddListener(() => PositionClicked(pin.PositionThreeButton.gameObject, pin, pin.pInfoContainers[2]));
			//p1.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;
			//p2.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;
			//p3.AddMissingComponent<ForwardTouch>().Clicked += PositionClicked;

			pin.pInfoContainers[0].transform.Find("Apply").GetComponent<Button>().onClick.AddListener(()=> ConfirmPosition());
            pin.pInfoContainers[1].transform.Find("Apply").GetComponent<Button>().onClick.AddListener(() => ConfirmPosition());
            pin.pInfoContainers[2].transform.Find("Apply").GetComponent<Button>().onClick.AddListener(() => ConfirmPosition());

			pin.pInfoContainers[0].SetActive(false);
            pin.pInfoContainers[1].SetActive(false);
            pin.pInfoContainers[2].SetActive(false);

            if (pin.gameObject.activeSelf)
				unlockedCompanies.Add(pin);
		}
	}

	void Start()
	{
		Button selectedButton = categories[GameBase.Ints.GetValue("LastCategory", 1) - 1];
		if(selectedButton != null)
		{
			selectedButton.onClick.Invoke();
		}
	}

	void PositionClicked(GameObject key, Company currentCompany, GameObject positionPanel)
	{
		CompanyPositionInfo cpi = togglePositionDictionary[key];
		currentCompany.pInfoContainers[0].SetActive(false);
        currentCompany.pInfoContainers[1].SetActive(false);
        currentCompany.pInfoContainers[2].SetActive(false);
        positionPanel.SetActive(true);

		/*
        if (_lastSelectedPosition != null)
		{
			bool samePosition = cpi == _lastSelectedPosition;
			ClosePosition();
			if(samePosition)
				return;
		}
		*/

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

		companyCloseDictionary[_lastSelectedPosition].SetActive(false);
		_lastSelectedPosition = null;
	}

	void CompanyClicked(GameObject key)
	{
		Debug.Log("company clicked");
		if (_lastSelectedCompany == null)
		{
			_lastSelectedCompany = toggleCompanyDictionay[key];
			foreach (Company c in unlockedCompanies)
			{
				GameObject button = c.mainButton.gameObject;
				if (button != key)
				{
					c.gameObject.SetActive(false);
					c.detailsPopup.SetActive(false);
				}
				else
				{
					c.detailsPopup.SetActive(true);
				}
			}
		}
		else
		{
			closeCompany();
		}
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

		foreach (Company c in unlockedCompanies)
		{
			c.gameObject.SetActive(true);
			c.detailsPopup.SetActive(false);
			c.pInfoContainers[0].SetActive(false);
            c.pInfoContainers[1].SetActive(false);
            c.pInfoContainers[2].SetActive(false);
        }
		
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

			Debug.Log(c.CompanyName + " " + listPosition);
			c.pInfoContainerTitles[listPosition-1].text = positionName;
			c.pInfoContainerDescs[listPosition-1].text = positionInfo;

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

