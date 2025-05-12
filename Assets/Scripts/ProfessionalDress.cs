using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

#pragma warning disable 0649
[System.Serializable]
public class ProfessionalDress : MonoBehaviour
{
	/// Variables ///

	#region Serialized Fields
	public List<EventDelegate> evaluateEvents = new List<EventDelegate>();

	[SerializeField]
	[Range(0f, 1f)]
	private float clothingAnimationProbability = .2f;
	[SerializeField]
	private Animator characterAnimator;
	[SerializeField]
	private RuntimeAnimatorController maleController;
	[SerializeField]
	private RuntimeAnimatorController femaleController;

	// All clothing items
	[SerializeField]
	private ClothingSetup hair = new ClothingSetup();
	[SerializeField]
	private ClothingSetup facialHair = new ClothingSetup();
	[SerializeField]
	private ClothingSetup top = new ClothingSetup();
	[SerializeField]
	private ClothingSetup bottom = new ClothingSetup();
	[SerializeField]
	private ClothingSetup shoes = new ClothingSetup();
	[SerializeField]
	private List<OtherSetup> other = new List<OtherSetup>();

	// Feedback charts
	[SerializeField]
	private List<TierCategoryFeedback> informalFB = new List<TierCategoryFeedback>();
	[SerializeField]
	private List<TierCategoryFeedback> casualFB = new List<TierCategoryFeedback>();
	[SerializeField]
	private List<TierCategoryFeedback> busCasFB = new List<TierCategoryFeedback>();
	[SerializeField]
	private List<TierCategoryFeedback> busProfFB = new List<TierCategoryFeedback>();



	// Clothing selection UI elements (toggles and grids)
	[SerializeField]
	private Clothing.Category.CategoryEnum startingGrid;
	[SerializeField]
	private GameObject faceToggle;
	[SerializeField]
	private HorizontalLayoutGroup faceGrid;
	[SerializeField]
	private TrackOnClick faceTrack;
	[SerializeField]
	private GameObject topToggle;
	[SerializeField]
	private HorizontalLayoutGroup topGrid;
	[SerializeField]
	private TrackOnClick topTrack;
	[SerializeField]
	private GameObject bottomToggle;
	[SerializeField]
	private HorizontalLayoutGroup bottomGrid;
	[SerializeField]
	private TrackOnClick bottomTrack;
	[SerializeField]
	private GameObject shoesToggle;
	[SerializeField]
	private HorizontalLayoutGroup shoesGrid;
	[SerializeField]
	private TrackOnClick shoesTrack;
	[SerializeField]
	private GameObject otherToggle;
	[SerializeField]
	private HorizontalLayoutGroup otherGrid;
	[SerializeField]
	private Button previous, next;

	[SerializeField]
	private TMP_Text clothingLabel;
	[SerializeField]
	private TMP_Text companyLabel;
	[SerializeField]
	private TMP_Text positionLabel;

	[SerializeField]
	private CrossSceneInfo.GenderEnum gender;
	[SerializeField]
	private Clothing.Tier.TierEnum positionTier;
	[SerializeField]
	private TMP_Text faceScore, faceBonus;
	[SerializeField]
	private TMP_Text faceFeedback;
	[SerializeField]
	private TMP_Text topScore, topBonus;
	[SerializeField]
	private TMP_Text topFeedback;
	[SerializeField]
	private TMP_Text bottomScore, bottomBonus;
	[SerializeField]
	private TMP_Text bottomFeedback;
	[SerializeField]
	private TMP_Text shoesScore, shoesBonus;
	[SerializeField]
	private TMP_Text shoesFeedback;
	[SerializeField]
	private TMP_Text otherScore, otherBonus;
	[SerializeField]
	private TMP_Text otherFeedback;

    [SerializeField]
	private GameObject zoomInButtom;
	[SerializeField]
	private GameObject zoomOutButtom;

	bool zoomed;
	public bool Zoomed { get { return zoomed; } set { zoomed = value; } }
	
	// Base toggle for clothing items
	[SerializeField]
	private GameObject baseClothesToggle;

	public BodyPartRender[] maleSetup = new BodyPartRender[0];
	public BodyPartRender[] femaleSetup = new BodyPartRender[0];
	#endregion

	#region Unserialized Fields
	private GameObject _currentGridToggle;
	private TrackOnClick _currentTrack;
	private HorizontalLayoutGroup _currentGrid;
	HorizontalLayoutGroup _currentHorizontalLayoutGroup;
	private GameObject _currentCenter;
	private Renderer[] _currentRenderers;
	// Clothing Label tweens
	List<UITweener> clTweens;

	// Renderers for each type of clothing item
	[System.NonSerialized]
	private Dictionary<string, Renderer> _otherRendererDictionary = new Dictionary<string, Renderer>();
	[System.NonSerialized]
	private Dictionary<string, Renderer> _otherRendererSecondaryDictionary = new Dictionary<string, Renderer>();

	// Clothing dictionary for toggle/clothing lookup
	[System.NonSerialized]
	private Dictionary<string, Clothing.Info> _clothingDictionary = new Dictionary<string, Clothing.Info>();
	// Clothing tier dictionary for tag/tier lookup (used for selected item tier comparison)
	[System.NonSerialized]
	private Dictionary<string, List<Clothing.Tier.TierEnum>> _clothingTierDictionary = new Dictionary<string, List<Clothing.Tier.TierEnum>>();
	// Toggle dictionary for image/toggle lookup (used when disabling previously selected clothing item)
	[System.NonSerialized]
	private Dictionary<Clothing.Info, SetActive> _toggleDictionary = new Dictionary<Clothing.Info, SetActive>();
    // Toggle button dictionary for image/toggle lookup (used when disabling previously selected clothing item)
    [System.NonSerialized]
    private Dictionary<Clothing.Info, Button> _toggleButtonDictionary = new Dictionary<Clothing.Info, Button>();
    // Toggle dictionary for category/activeClothing lookup (used when disabling previously selected clothing item)
    [System.NonSerialized]
	private Dictionary<Toggle, Clothing.Info> _toggleClothingDictionary = new Dictionary<Toggle, Clothing.Info>();
	
	[System.NonSerialized]
	private Clothing.Info _activeHair, _activeFacialHair, _activeTop, _activeBottom, _activeShoes;
	[System.NonSerialized]
	private List<Clothing.Info> _activeOther = new List<Clothing.Info>();

	private GameObject startingHair;
	#endregion
	
	/// Functions ///

// Prevent bug from occurring when resuming game after suspend in iOS & Android
#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
	void OnApplicationPause(bool pauseStatus) {
		if(!pauseStatus)
			Application.LoadLevel("Start Screen");
	}
#endif

	#region Setup
	// Populate the grids and set up interactions
	void Awake()
	{
		startingHair = null;
		clTweens = clothingLabel.GetComponents<UITweener>().ToList();

		if(CrossSceneInfo.SelectedCompany != null)
		{

			companyLabel.text = CrossSceneInfo.SelectedCompany.CompanyName;
			positionLabel.text = CrossSceneInfo.SelectedCompany.PositionName;
			positionTier = CrossSceneInfo.SelectedCompany.PositionDressTier;
		}
		

		if(gender == CrossSceneInfo.GenderEnum.MALE)
		{
			foreach(BodyPartRender bpr in maleSetup)
			{
				bpr.material.color = GlobalData.currentCharacterSelection.skinColor;
				bpr.renderer.sharedMaterial = bpr.material;
			}
			characterAnimator.runtimeAnimatorController = maleController;
		}
		else
		{
			foreach(BodyPartRender bpr in femaleSetup)
			{
				bpr.material.color = GlobalData.currentCharacterSelection.skinColor;
				bpr.renderer.sharedMaterial = bpr.material;
			}
			characterAnimator.runtimeAnimatorController = femaleController;
		}

		// Tie active clothing to the appropriate catagory
		//_toggleClothingDictionary.Add(topToggle, _activeTop);
		//_toggleClothingDictionary.Add(bottomToggle, _activeBottom);
		//_toggleClothingDictionary.Add(shoesToggle, _activeShoes);

		bool male = gender == CrossSceneInfo.GenderEnum.MALE;
		hair.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		facialHair.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		top.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		bottom.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		shoes.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		foreach(OtherSetup os in other)
		{
			os.clothing.RemoveAll(c=> c.Gender != Clothing.Info.GenderClass.EITHER && (c.Gender == Clothing.Info.GenderClass.MALE) != male );
		}

	
		// Assign toggles for each clothing item, setup "on click" actions, and position items correctly
		foreach(Clothing.Info c in hair.clothing)
		{
			if(startingHair == null)
				startingHair = Setup (c, faceGrid.gameObject, ()=> { HairItem(c.Tag); });
			else
				Setup (c, faceGrid.gameObject, () => HairItem(c.Tag) );
        }
		foreach(Clothing.Info c in facialHair.clothing)
			Setup (c, faceGrid.gameObject, () => { FacialHairItem(c.Tag); });

		foreach (Clothing.Info c in top.clothing)
			Setup(c, topGrid.gameObject, () => TopItem(c.Tag) );
		
		foreach(Clothing.Info c in bottom.clothing)
			Setup (c, bottomGrid.gameObject, () => BottomItem(c.Tag));
		
		foreach(Clothing.Info c in shoes.clothing)
			Setup (c, shoesGrid.gameObject, () => ShoesItem(c.Tag));
		
		foreach(OtherSetup aS in other)
		{
			foreach(Clothing.Info c in aS.clothing)
			{
				 Setup (c, otherGrid.gameObject, () => OtherItem(c.Tag));
				_otherRendererDictionary.Add(c.Tag, aS.renderer);
				if(aS.otherRenderer != null)
					_otherRendererSecondaryDictionary.Add(c.Tag, aS.otherRenderer);
			}
		}
		

		// Set all category grid toggle and track click interactions
		faceToggle.GetComponent<Button>().onClick.AddListener(FaceToggleClicked);
		faceTrack.AddToOnClick(()=> TrackClicked(faceTrack, faceToggle.GetComponent<Button>()));

        topToggle.GetComponent<Button>().onClick.AddListener(TopToggleClicked);
        topTrack.AddToOnClick(() => TrackClicked(topTrack, topToggle.GetComponent<Button>()));

        bottomToggle.GetComponent<Button>().onClick.AddListener(BottomToggleClicked);
        bottomTrack.AddToOnClick(() => TrackClicked(bottomTrack, bottomToggle.GetComponent<Button>()));

        shoesToggle.GetComponent<Button>().onClick.AddListener(ShoesToggleClicked);
        shoesTrack.AddToOnClick(() => TrackClicked(shoesTrack, shoesToggle.GetComponent<Button>()));

        otherToggle.GetComponent<Button>().onClick.AddListener(OtherToggleClicked);

		zoomInButtom = GameObject.Find("InspectZoomButton");
        zoomOutButtom = GameObject.Find("InspectBackButton");

		zoomOutButtom.SetActive(false);
    }


    // Setup the toggle, direct the interaction, and include in the appropriate dictionaries
    GameObject Setup(Clothing.Info c, GameObject grid, UnityAction action, bool setActive = false)
	{
		//ButtonStateToggle cToggle = NGUITools.AddChild(grid, baseClothesToggle.gameObject).GetComponent<ButtonStateToggle>();

		GameObject newToggle = Instantiate(baseClothesToggle, grid.transform);
		Clothing.Info thisC = c;
        newToggle.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = thisC.Image;
		
		newToggle.GetComponent<Button>().onClick.AddListener(action);
		
		_clothingDictionary.Add(thisC.Tag, thisC);
		_clothingTierDictionary.Add(thisC.Tag, thisC.Tiers);
		_toggleDictionary.Add(thisC, newToggle.transform.GetChild(1).GetComponent<SetActive>());
		_toggleButtonDictionary.Add(thisC, newToggle.GetComponent<Button>());
		if (setActive)
			newToggle.GetComponent<Button>().onClick.Invoke();

		return newToggle;
	}


	// What to do when a tracker is clicked
	void TrackClicked(TrackOnClick tracker, Button touch)
	{
		// If zoom is already toggled
		if(zoomed)
		{
			// Reset zoom if you're already zoomed into this category
			if(_currentTrack == tracker)
			{
				zoomOutButtom.GetComponent<Button>().onClick.Invoke();
			}
			// Select this category (will automatically zoom in)
			else
			{
				touch.onClick.Invoke();
			}
		} 
		// If zoom isn't toggled
		else
		{
            // Select this category
            touch.onClick.Invoke();
            // and zoom in
            zoomInButtom.GetComponent<Button>().onClick.Invoke();
        }
	}


	// Establish starting category
	void Start()
	{
		faceToggle.GetComponent<CategoryToggle>().SetToOff();
		topToggle.GetComponent<CategoryToggle>().SetToOff();
		bottomToggle.GetComponent<CategoryToggle>().SetToOff();
		shoesToggle.GetComponent<CategoryToggle>().SetToOff();
		otherToggle.GetComponent<CategoryToggle>().SetToOff();
		// Open the default clothes category tab
		switch(startingGrid)
		{
			case Clothing.Category.CategoryEnum.HEAD:
				setCurrent(faceToggle, faceTrack, faceGrid, null);
				previous.gameObject.SetActive(false);
				next.gameObject.SetActive(false);
				break;
			case Clothing.Category.CategoryEnum.TOP:
				setCurrent(topToggle, topTrack, topGrid, top.renderers);
				break;
			case Clothing.Category.CategoryEnum.BOTTOM:
				setCurrent(bottomToggle, bottomTrack, bottomGrid, bottom.renderers);
				break;
			case Clothing.Category.CategoryEnum.SHOES:
				setCurrent(shoesToggle, shoesTrack, shoesGrid, shoes.renderers);
				break;
			case Clothing.Category.CategoryEnum.OTHER:
				setCurrent(otherToggle, null, otherGrid, null);
				
				zoomInButtom.gameObject.SetActive(false);
				zoomOutButtom.gameObject.SetActive(false);

				previous.gameObject.SetActive(false);
				next.gameObject.SetActive(false);
				break;
		}

		_currentGridToggle.GetComponent<CategoryToggle>().SetToOn();
		
		previous.onClick.AddListener(Previous);
		next.onClick.AddListener(Next);

		startingHair.GetComponent<Button>().onClick.Invoke();
 	}
    #endregion

    #region Clothing Handle	
    // What to do when the other toggle is clicked
    public void OtherToggleClicked()
	{
		if(_currentGridToggle != otherToggle) 
		{
			otherToggle.GetComponent<CategoryToggle>().SetToOn();
			_currentGridToggle.GetComponent<CategoryToggle>().SetToOff();

			setCurrent(otherToggle, null, otherGrid, null);
			previous.gameObject.SetActive(false);
			next.gameObject.SetActive(false);
            zoomInButtom.gameObject.SetActive(false);
            zoomOutButtom.gameObject.SetActive(false);
        }

		if (zoomed)
			zoomInButtom.GetComponent<Button>().onClick.Invoke();
	}
	void OtherItem(string tag)
	{
		// Get the clothing item that corresponds with the selected toggle
		Clothing.Info c = _clothingDictionary[tag]; 
		// Get the renderer that corresponds with the selected toggle
		Renderer s = _otherRendererDictionary[tag];

		// Remove a conflicting other if one exists
		if(s.material.mainTexture != null)
		{
			Clothing.Info activeCI = _activeOther.FirstOrDefault(ci=>ci.Pieces[0] == s.material.mainTexture);
			if(c.Pieces[0] != s.material.mainTexture)
			{
				if(activeCI != null)
				{
					SetActive t = _toggleDictionary[activeCI];
					t.False();
				}
				// Apply the selected other
				s.material.mainTexture = c.Pieces[0];
				if(_otherRendererSecondaryDictionary.ContainsKey(tag))
					_otherRendererSecondaryDictionary[tag].material.mainTexture = c.Pieces[0];
				// Add the other to the list of active others
				_activeOther.Add(c);
				clothingLabel.text = c.DisplayTag;
				
				foreach(UITweener tween in clTweens)
					tween.PlayForward_FromBeginning();
			} else
			{
				s.material.mainTexture = null;
				if(_otherRendererSecondaryDictionary.ContainsKey(tag))
					_otherRendererSecondaryDictionary[tag].material.mainTexture = null;
			}
			if(activeCI != null)
			{
				SetActive t = _toggleDictionary[activeCI];
				_activeOther.Remove(_clothingDictionary[activeCI.Tag]);
			}
		} else
		{
			// Apply the selected other
			s.material.mainTexture = c.Pieces[0];
			if(_otherRendererSecondaryDictionary.ContainsKey(tag))
				_otherRendererSecondaryDictionary[tag].material.mainTexture = c.Pieces[0];
			
			// Add the other to the list of active others
			_activeOther.Add(c);

			clothingLabel.text = c.DisplayTag;
			
			foreach(UITweener tween in clTweens)
				tween.PlayForward_FromBeginning();
		}
	}


	void setToggle(GameObject catToggle, TrackOnClick toc, HorizontalLayoutGroup grid, Renderer[] renderers, Clothing.Info c)
	{
		if(_currentGridToggle != catToggle) 
		{
            catToggle.GetComponent<CategoryToggle>().SetToOn();
            _currentGridToggle.GetComponent<CategoryToggle>().SetToOff();

            setCurrent(catToggle, toc, grid, renderers);

			previous.gameObject.SetActive(true);
			next.gameObject.SetActive(true);
            zoomInButtom.gameObject.SetActive(true);
            zoomOutButtom.gameObject.SetActive(false);

            if (zoomed)
				zoomOutButtom.GetComponent<Button>().onClick.Invoke();
		}
	}
	
	void setItem(string tag, ref Clothing.Info clothing, Renderer[] renderers)
	{
		Clothing.Info c = _clothingDictionary[tag];
		
		if(clothing != c)
		{
			if(clothing != null)
				_toggleDictionary[clothing].False();

			for(int i = 0; i < renderers.Length; i++) {
				if(i >= c.Pieces.Length)
					renderers[i].material.mainTexture = null;
				else
					renderers[i].material.mainTexture = c.Pieces[i];
			}
			clothing = c;
			clothingLabel.text = c.DisplayTag;

			foreach(UITweener tween in clTweens)
				tween.PlayForward_FromBeginning();
		} else
		{
			foreach(Renderer r in renderers)
				r.material.mainTexture = null;
			clothing = null;
		}
	}

    void setCurrent(GameObject catToggle, TrackOnClick toc, HorizontalLayoutGroup grid, Renderer[] renderers)
    {
		_currentGrid = grid;
        _currentGridToggle = catToggle;
        _currentTrack = toc;
        _currentHorizontalLayoutGroup = grid;
        _currentRenderers = renderers;
    }
    #endregion

    #region Inputs
    // What to do when the face toggle is clicked
    public void FaceToggleClicked()
	{
		if(_currentGridToggle != faceToggle) 
		{
			faceToggle.GetComponent<CategoryToggle>().SetToOn();
			_currentGridToggle.GetComponent<CategoryToggle>().SetToOff();
			
			setCurrent(faceToggle, faceTrack, faceGrid, null);
			previous.gameObject.SetActive(false);
			next.gameObject.SetActive(false);

			zoomInButtom.gameObject.SetActive(true);
            zoomOutButtom.gameObject.SetActive(false);
        }

		if (zoomed)
			zoomOutButtom.GetComponent<Button>().onClick.Invoke();
	}
	void HairItem(string tag)
	{
		setItem(tag, ref _activeHair, hair.renderers);
		if(gender == CrossSceneInfo.GenderEnum.FEMALE)
			PlayAnimation("Hair Changed");
	}
	void FacialHairItem(string tag)
	{
		setItem(tag, ref _activeFacialHair, facialHair.renderers);
	}


    // What to do when the top toggle is clicked
    public void TopToggleClicked()
	{
		setToggle(topToggle, topTrack, topGrid, top.renderers, _activeTop);
	}
	void TopItem(string tag)
	{
		setItem(tag, ref _activeTop, top.renderers);
		if(gender == CrossSceneInfo.GenderEnum.MALE)
			characterAnimator.SetBool("Shirt Wipe", UnityEngine.Random.Range(0f, 1f) > .5f);
		PlayAnimation("Shirt Changed");
	}


    // What to do when the bottom toggle is clicked
    public void BottomToggleClicked()
	{
		setToggle(bottomToggle, bottomTrack, bottomGrid, bottom.renderers, _activeBottom);
	}
	void BottomItem(string tag)
	{
		setItem(tag, ref _activeBottom, bottom.renderers);
		if(gender == CrossSceneInfo.GenderEnum.FEMALE)
			characterAnimator.SetBool("Skirt", _activeBottom != null && _activeBottom.Pieces[5] != null);
		PlayAnimation("Pants Changed");
	}


    // What to do when the shoes toggle is clicked
    public void ShoesToggleClicked()
	{
		setToggle(shoesToggle, shoesTrack, shoesGrid, shoes.renderers, _activeShoes);
	}
	void ShoesItem(string tag)
	{
		setItem(tag, ref _activeShoes, shoes.renderers);
		PlayAnimation("Shoes Changed");
	}


	void PlayAnimation(string trigger)
	{
		if(UnityEngine.Random.Range(0f, 1f) > clothingAnimationProbability || characterAnimator.GetBool("Playing Animation"))
			return;

		characterAnimator.SetBool("Playing Animation", true);
		characterAnimator.SetTrigger(trigger);
	}	

	// Zoom in on the current category's tracker
	public void Zoom()
	{
		if(_currentTrack != null)
			_currentTrack.Focus();
	}


	// Goes to the next item in the category (goes to first item if at end)
	void Next()
	{
		if(_currentRenderers == null || _currentRenderers.Length == 0 || _currentGrid.transform.childCount == 0)
			return;

		int index = -1;

		Clothing.Info activeClothing = _activeTop;

		if(_currentGridToggle == bottomToggle)
			activeClothing = _activeBottom;
		else if(_currentGridToggle == shoesToggle)
			activeClothing = _activeShoes;

		if(activeClothing != null)
		{
			//SetActive bst = _toggleDictionary[activeClothing];
			//bst.False();

			Button button = _toggleButtonDictionary[activeClothing];
			button.onClick.Invoke();
			index = button.transform.GetSiblingIndex();
			
			if(_currentGrid.transform.childCount == 1)
				return;
		}

		index++;
		if(index == _currentGrid.transform.childCount)
			index = 0;

		_currentCenter = _currentGrid.transform.GetChild(index).gameObject;
        _currentCenter.transform.GetChild("Check").GetComponentInChildren<SetActive>().False();
        _currentCenter.GetComponent<Button>().onClick.Invoke();

        _currentGrid.transform.parent.parent.GetComponent<CenterItemInScrollrect>().CenterOnItem(_currentCenter.GetComponent<RectTransform>());
        //center.CenterOn(_currentCenter.transform);
        //center.enabled = true;
    }


	// Goes to the previous item in the category (goes to end if at first item)
	void Previous()
	{
		if(_currentRenderers == null || _currentRenderers.Length == 0 || _currentGrid.transform.childCount == 0)
			return;
		
		int index = _currentGrid.transform.childCount;

		Clothing.Info activeClothing = _activeTop;

		if(_currentGridToggle == bottomToggle)
			activeClothing = _activeBottom;
		else if(_currentGridToggle == shoesToggle)
			activeClothing = _activeShoes;

		if(activeClothing != null)
		{
			//SetActive bst = _toggleDictionary[activeClothing];
			//bst.False();

			Button button = _toggleButtonDictionary[activeClothing];
			button.onClick.Invoke();
			index = button.transform.GetSiblingIndex();
			
			if(_currentGrid.transform.childCount == 1)
				return;
		}
		
		index--;
		if(index == -1)
			index = _currentGrid.transform.childCount - 1;

        _currentCenter = _currentGrid.transform.GetChild(index).gameObject;
        _currentCenter.transform.GetChild("Check").GetComponentInChildren<SetActive>().False();
        _currentCenter.GetComponent<Button>().onClick.Invoke();

		_currentGrid.transform.parent.parent.GetComponent<CenterItemInScrollrect>().CenterOnItem(_currentCenter.GetComponent<RectTransform>());
        //center.CenterOn(_currentCenter.transform);
        //center.enabled = true;
    }
	#endregion 

	#region Analysis
	// Evaluate the clothing selection
	public void Evaluate()
	{
		EventDelegate.Execute(evaluateEvents);

		List<TierCategoryFeedback> feedback = new List<TierCategoryFeedback>();
		switch(positionTier)
		{
		case Clothing.Tier.TierEnum.INFORMAL:
			feedback = informalFB;
			break;
		case Clothing.Tier.TierEnum.CASUAL:
			feedback = casualFB;
			break;
		case Clothing.Tier.TierEnum.BUSINESS_CASUAL:
			feedback = busCasFB;
			break;
		case Clothing.Tier.TierEnum.BUSINESS_PROFESSIONAL:
			feedback = busProfFB;
			break;
		}

		
		int highScore = 0;
		int hsIndex = -1;
		
		int _faceScore = 100;
		float _faceBonus = 0;
		float hairMult = (_activeFacialHair != null) ? .5f : 1f;
		if(_activeHair != null)
		{
			Debug.Log("has hair or hat");
			_faceScore = 0;
			if(itemColorCheck(_activeHair))
				_faceBonus += 5f * hairMult;

			if(itemDesignCheck(_activeHair))
				_faceBonus += 5f * hairMult;

			highScore = -1;
			hsIndex = -1;
			for(int i=0; i<_activeHair.Tiers.Count; i++)
			{
				if(Clothing.Tier.Score(_activeHair.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(_activeHair.Tiers[i], positionTier);
					hsIndex = i;
				}
			}
			highScore = Mathf.Max(highScore, 0);

			_faceScore += Mathf.CeilToInt(hairMult * highScore) ;

			if(hsIndex != -1)
			{
				TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Head" && fb.tiers.Contains(_activeHair.Tiers[hsIndex]));
				if(tcf != null)
					faceFeedback.text = tcf.feedback;
			}
		}
		if(_activeFacialHair != null)
		{
			if(_activeHair == null)
				_faceScore = 50;
			if(itemColorCheck(_activeFacialHair))
				_faceBonus += 5f * hairMult;
			
			if(itemDesignCheck(_activeFacialHair))
				_faceBonus += 5f * hairMult;
			
			highScore = -1;
			hsIndex = -1;
			for(int i=0; i<_activeFacialHair.Tiers.Count; i++)
			{
				if(Clothing.Tier.Score(_activeFacialHair.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(_activeFacialHair.Tiers[i], positionTier);
					hsIndex = i;
				}
			}
			highScore = Mathf.Max(highScore, 0);
			_faceScore += Mathf.CeilToInt(hairMult * highScore);
			
			if(hsIndex != -1)
			{
				TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Face" && fb.tiers.Contains(_activeFacialHair.Tiers[hsIndex]));
				if(tcf != null)
				{
					if(_activeHair == null)
						faceFeedback.text = tcf.feedback;
					else
						faceFeedback.text += "\n\n" + tcf.feedback;
				}
			}
		}
		
		int _topScore = -100;
		int _topBonus = 0;
		if(_activeTop != null)
		{
			_topScore = 0;
			if(itemColorCheck(_activeTop))
				_topBonus += 5;
			
			if(itemDesignCheck(_activeTop))
				_topBonus += 5;
			
			highScore = -1;
			hsIndex = -1;
			for(int i=0; i<_activeTop.Tiers.Count; i++)
			{
				if(Clothing.Tier.Score(_activeTop.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(_activeTop.Tiers[i], positionTier);
					hsIndex = i;
				}
			}
			highScore = Mathf.Max(highScore, 0);
			_topScore += highScore;
			
			if(hsIndex != -1)
			{
				TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Top" && fb.tiers.Contains(_activeTop.Tiers[hsIndex]));
				if(tcf != null)
					topFeedback.text = tcf.feedback;
			}
		}

		int _bottomScore = -100;
		int _bottomBonus = 0;
		if(_activeBottom != null)
		{
			_bottomScore = 0;
			if(itemColorCheck(_activeBottom))
				_bottomBonus += 5;
			
			if(itemDesignCheck(_activeBottom))
				_bottomBonus += 5;
			
			highScore = -1;
			hsIndex = -1;
			for(int i=0; i<_activeBottom.Tiers.Count; i++)
			{
				if(Clothing.Tier.Score(_activeBottom.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(_activeBottom.Tiers[i], positionTier);
					hsIndex = i;
				}
			}
			highScore = Mathf.Max(highScore, 0);
			_bottomScore += highScore;
			
			if(hsIndex != -1)
			{
				TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Bottom" && fb.tiers.Contains(_activeBottom.Tiers[hsIndex]));
				if(tcf != null)
					bottomFeedback.text = tcf.feedback;
			}
		}

		int _shoesScore = -100;
		int _shoesBonus = 0;
		if(_activeShoes != null)
		{
			_shoesScore = 0;
			if(itemColorCheck(_activeShoes))
				_shoesBonus += 5;
			
			if(itemDesignCheck(_activeShoes))
				_shoesBonus += 5;
			
			highScore = -1;
			hsIndex = -1;
			for(int i=0; i<_activeShoes.Tiers.Count; i++)
				if(Clothing.Tier.Score(_activeShoes.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(_activeShoes.Tiers[i], positionTier);
					hsIndex = i;
			}
			highScore = Mathf.Max(highScore, 0);
			_shoesScore += highScore;
			
			if(hsIndex != -1)
			{
				TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Shoes" && fb.tiers.Contains(_activeShoes.Tiers[hsIndex]));
				if(tcf != null)
					shoesFeedback.text = tcf.feedback;
			}
		}
		
		int _itemScore = 75;
		float _itemBonus = 0;
		if(_activeOther.Count != 0)
		{
			_itemScore = 0;
			float itemMult = 1f / _activeOther.Count;
			foreach(Clothing.Info other in _activeOther)
			{
				if(itemColorCheck(other))
					_itemBonus += 5f * itemMult;
				
				if(itemDesignCheck(other))
					_itemBonus += 5f * itemMult;
				
				highScore = -1;
				hsIndex = -1;
				for(int i=0; i<other.Tiers.Count; i++)
					if(Clothing.Tier.Score(other.Tiers[i], positionTier) > highScore)
				{
					highScore = Clothing.Tier.Score(other.Tiers[i], positionTier);
					hsIndex = i;
				}
				highScore = Mathf.Max(highScore, 0);
				
				_itemScore += Mathf.CeilToInt(highScore * itemMult) ;
				
				if(hsIndex != -1)
				{
					TierCategoryFeedback tcf = feedback.FirstOrDefault(fb=> fb.gender == gender && Clothing.Category.GetName(fb.category) == "Other" && fb.tiers.Contains(other.Tiers[hsIndex]));
					if(tcf != null)
						otherFeedback.text = tcf.feedback;
				}
			}
		}

		int _score = _faceScore + Mathf.CeilToInt(_faceBonus) + _topScore + _topBonus +
					_bottomScore + _bottomBonus + _shoesScore + _shoesBonus + _itemScore + Mathf.CeilToInt(_itemBonus);

		// Only record the score if you have passed
		bool passed = false;
		if(CrossSceneInfo.SelectedCompany != null && _score >= CrossSceneInfo.PassingCuttoff)
		{
			passed = true;
			Debug.Log("recording score");
			CrossSceneInfo.SetScore(CrossSceneInfo.SelectedCompany.CompanyName +"_"+CrossSceneInfo.SelectedCompany.PositionName, _score);
		}
		
		faceScore.text = _faceScore.ToString();
		faceBonus.text = "+" + Mathf.CeilToInt(_faceBonus).ToString();
		topScore.text = _topScore.ToString();
		topBonus.text = "+" + _topBonus.ToString();
		bottomScore.text = _bottomScore.ToString();
		bottomBonus.text = "+" + _bottomBonus.ToString();
		shoesScore.text = _shoesScore.ToString();
		shoesBonus.text = "+" + _shoesBonus.ToString();
		otherScore.text = _itemScore.ToString();
		otherBonus.text = "+" + Mathf.CeilToInt(_itemBonus).ToString();
		

        /*
		var dataEvent = new SkillEvent();
		dataEvent.AddString("EventType", "FinishedLevel");
		dataEvent.AddNumber("FaceScore", _faceScore);
		dataEvent.AddNumber("TopScore", _topScore);
		dataEvent.AddNumber("BottomScore", _bottomScore);
		dataEvent.AddNumber("ShoesScore", _shoesScore);
		dataEvent.AddNumber("OtherScore", _itemScore);
		dataEvent.AddNumber("TotalScore", _score);
		dataEvent.Save();
		*/

        TimeSpan timeElapsed = (CrossSceneInfo.LevelStartedTimeStamp - DateTime.Now).Duration();
		float secondsElapsed = (float) timeElapsed.TotalSeconds;
		EventRecorder.RecordLevelCompleted(CrossSceneInfo.LevelAttemptId, CrossSceneInfo.LastCompanyName, CrossSceneInfo.LastPositionName, passed, _faceScore, _topScore, _bottomScore, _shoesScore, _itemScore, _score, CrossSceneInfo.PassingCuttoff, secondsElapsed);
	}

	bool itemColorCheck(Clothing.Info item)
	{
		bool match = false;
		bool itemContainsAll = item.MatchingColors.Contains("All");

		if(_activeHair != null && item != _activeHair)
		{
			int colorMatchCheck = checkMatch(item.MatchingColors, _activeHair.Colors, itemContainsAll);
			if(colorMatchCheck == 1)
				match = true;
			else if(colorMatchCheck == 2)
				return false;
		}
		if(_activeFacialHair != null && item != _activeFacialHair)
		{
			int colorMatchCheck = checkMatch(item.MatchingColors, _activeFacialHair.Colors, itemContainsAll);
			if(colorMatchCheck == 1)
				match = true;
			else if(colorMatchCheck == 2)
				return false;
		}
		if(_activeTop != null && item != _activeTop)
		{
			int colorMatchCheck = checkMatch(item.MatchingColors, _activeTop.Colors, itemContainsAll);
			if(colorMatchCheck == 1)
				match = true;
			else if(colorMatchCheck == 2)
				return false;
		}
		if(_activeBottom != null && item != _activeBottom)
		{
			int colorMatchCheck = checkMatch(item.MatchingColors, _activeBottom.Colors, itemContainsAll);
			if(colorMatchCheck == 1)
				match = true;
			else if(colorMatchCheck == 2)
				return false;
		}
		if(_activeShoes != null && item != _activeFacialHair)
		{
			int colorMatchCheck = checkMatch(item.MatchingColors, _activeShoes.Colors, itemContainsAll);
			if(colorMatchCheck == 1)
				match = true;
			else if(colorMatchCheck == 2)
				return false;
		}

		return match;
	}
	
	bool itemDesignCheck(Clothing.Info item)
	{
		bool match = false;
		bool itemContainsAll = item.MatchingDesigns.Contains("All");
		
		if(_activeHair != null && item != _activeHair)
		{
			int designMatchCheck = checkMatch(item.MatchingDesigns, _activeHair.Designs, itemContainsAll);
			if(designMatchCheck == 1)
				match = true;
			else if(designMatchCheck == 2)
				return false;
		}
		if(_activeFacialHair != null && item != _activeFacialHair)
		{
			int designMatchCheck = checkMatch(item.MatchingDesigns, _activeFacialHair.Designs, itemContainsAll);
			if(designMatchCheck == 1)
				match = true;
			else if(designMatchCheck == 2)
				return false;
		}
		if(_activeTop != null && item != _activeTop)
		{
			int designMatchCheck = checkMatch(item.MatchingDesigns, _activeTop.Designs, itemContainsAll);
			if(designMatchCheck == 1)
				match = true;
			else if(designMatchCheck == 2)
				return false;
		}
		if(_activeBottom != null && item != _activeBottom)
		{
			int designMatchCheck = checkMatch(item.MatchingDesigns, _activeBottom.Designs, itemContainsAll);
			if(designMatchCheck == 1)
				match = true;
			else if(designMatchCheck == 2)
				return false;
		}
		if(_activeShoes != null && item != _activeShoes)
		{
			int designMatchCheck = checkMatch(item.MatchingDesigns, _activeShoes.Designs, itemContainsAll);
			if(designMatchCheck == 1)
				match = true;
			else if(designMatchCheck == 2)
				return false;
		}
		
		return match;
	}

	// Returns 0 if there is no match, 2 if there is a bad match, and 1 if there are no bad matches but at least 1 good match
	int checkMatch(string[] matches, string[] qualities, bool matchesAll)
	{
		int match = 0;
		foreach(string quality in qualities)
		{
			if(matchesAll)
			{
				if(matches.Contains("-"+quality))
					return 2;
				else
					match = 1;
			}
			else if(matches.Contains(quality))
				match = 1;
			else if(matches.Contains("-"+quality))
				return 2;
		}

		return match;
	}
	#endregion 
	
	#region Importing
	#if UNITY_EDITOR
	public void ParseClothes(TextAsset CSV, bool clearItemsOnImport)
	{
		if(CSV == null)
			return;

		if(clearItemsOnImport)
		{
			hair.clothing = new List<Clothing.Info>();
			facialHair.clothing = new List<Clothing.Info>();
			top.clothing = new List<Clothing.Info>();
			bottom.clothing = new List<Clothing.Info>();
			shoes.clothing = new List<Clothing.Info>();
			other = new List<OtherSetup>();
		}

		string[] items = CSV.text.Split(new char[]{'\n'});

		for(int i = 1; i < items.Length; i++)
		{
			string[] fields = items[i].Split(new char[]{'\t'});
			Clothing.Info c = null;;
			
			string area = fields[0];
			ClothingSetup cs = null;
			OtherSetup os = null;

			switch(area)
			{
			case "Hair":
				cs = hair;
				break;
			case "Face":
				cs = facialHair;
				break;
			case "Top":
				cs = top;
				break;
			case "Bottom":
				cs = bottom;
				break;
			case "Shoes":
				cs = shoes;
				break;
			default:
				os = other.FirstOrDefault(o=>o.category == area);
				if(os == null)
				{
					other.Add(new OtherSetup());
					os = other.Last();
					os.category = area;
				}
				break;
			}

			if(cs != null)
				c = setupClothing(cs, area, fields);
			else if(os != null)
				c = setupOther(os, fields);
			else
				continue;

			string gender = fields[1];
			if(gender != "")
				c.Gender = (gender == "Male") ? Clothing.Info.GenderClass.MALE : (gender == "Female") ? Clothing.Info.GenderClass.FEMALE : Clothing.Info.GenderClass.EITHER;
			else
				c.Gender = Clothing.Info.GenderClass.EITHER;
			
			string displayTag = fields[12];
			c.DisplayTag = displayTag;

			string[] tiers = fields[5].Split(new char[]{';'}).Trim();
			foreach(string tier in tiers)
				c.Tiers.Add(Clothing.Tier.GetEnum(tier));

			string colors = fields[6];
			if(colors != "")
			{
				c.Colors = colors.Split(new char[]{';'}).Trim();
			}

			string matchingColors = fields[9];
			if(matchingColors != "")
			{
				c.MatchingColors = matchingColors.Split(new char[]{';'}).Trim();
			}
			
			string designs = fields[4];
			if(designs != "")
			{
				c.Designs = designs.Split(new char[]{';'}).Trim();
			}
			
			string matchingDesigns = fields[10];
			if(matchingDesigns != "")
			{
				c.MatchingDesigns = matchingDesigns.Split(new char[]{';'}).Trim();
			}

			string goodMatches = fields[7];
			if(goodMatches != "")
				c.GoodMatches = goodMatches.Split(new char[]{';'});
			else c.GoodMatches = new string[0];
			
			string badMatches = fields[8];
			if(badMatches != "")
				c.BadMatches = badMatches.Split(new char[]{';'});
			else c.BadMatches = new string[0];

			Texture[] icon = getPieces(new string[]{fields[11]}, ".png", ".tif");
			if(icon.Length > 0)
				c.Image = icon[0];
		}
	}

	Clothing.Info setupClothing(ClothingSetup cs, string area, string[] fields)
	{
		string tag = fields[3];
		Clothing.Info c = cs.clothing.FirstOrDefault(fod => fod.Tag == tag);
		if(c == null){
			cs.clothing.Add(new Clothing.Info());
			c = cs.clothing.Last();
			c.Tag = tag;
		}
		
		c.Pieces = new Texture[cs.renderers.Length];
		Texture[] pieces = getPieces(fields[2].Split(new char[]{';'}).Trim(), ".tif", ".png");

		if(pieces.Length != 0)
		{
			switch(area)
			{
			case "Hair":
				c.Pieces = pieces;
				break;
			case "Face":
				c.Pieces = pieces;
				break;
			case "Top":
				c.Pieces[0] = pieces[0];
				if(pieces.Length > 1)
				{
					c.Pieces[1] = pieces[1];
					c.Pieces[2] = pieces[1];
					if(pieces.Length > 2)
					{
						c.Pieces[3] = pieces[2];
						c.Pieces[4] = pieces[2];
					}
				}
				break;
			case "Bottom":
				c.Pieces[0] = pieces[0];
				if(pieces.Length > 1)
				{
					c.Pieces[1] = pieces[1];
					c.Pieces[2] = pieces[1];
					if(pieces.Length > 2)
					{
						c.Pieces[3] = pieces[2];
						c.Pieces[4] = pieces[2];
						if(pieces.Length > 3)
							c.Pieces[5] = pieces[3];
					}
				}
				break;
			case "Shoes":
				c.Pieces[0] = pieces[0];
				c.Pieces[1] = pieces[0];
				break;
			}
		}

		return c;
	}

	Clothing.Info setupOther(OtherSetup os, string[] fields)
	{
		string tag = fields[3];
		Clothing.Info c = os.clothing.FirstOrDefault(fod => fod.Tag == tag);
		if(c == null){
			os.clothing.Add(new Clothing.Info());
			c = os.clothing.Last();
			c.Tag = tag;
		}
		
		c.Pieces = new Texture[1];
		Texture[] pieces = getPieces(fields[2].Split(new char[]{';'}).Trim(), ".tif", ".png");
		if(pieces.Length != 0)
			c.Pieces[0] = pieces[0];

		return c;
	}
	
	Texture[] getPieces(string[] pieceNames, string extension, string fallbackExtension)
	{
		Texture[] pieces = new Texture[pieceNames.Length];

		for(int i = 0; i < pieceNames.Length; i++)
		{
			string search = "Assets/Models/Textures/" + pieceNames[i];
			pieces[i] = AssetDatabase.LoadAssetAtPath<Texture>(search + extension);
			if(pieces[i] == null)
			{
				Debug.Log("couldn't find " + pieceNames[i] + extension);
				pieces[i] = AssetDatabase.LoadAssetAtPath<Texture>(search + fallbackExtension);
				if(pieces[i] == null)
					Debug.Log("couldn't find " + pieceNames[i] + fallbackExtension);
				else
					Debug.Log("FOUND " + pieceNames[i] + fallbackExtension);
			} else
				Debug.Log("FOUND " + pieceNames[i] + extension);
		}

		return pieces;
	}

	public void ParseFeedback(TextAsset TSV, bool male, bool clearItemsOnImport)
	{
		if(TSV == null)
			return;
		
		if(clearItemsOnImport)
		{
			ClearFeedback();
		}
		
		string[] items = TSV.text.Split(new char[]{'\n'});
		
		for(int i = 1; i < items.Length; i++)
		{
			string[] fields = items[i].Split(new char[]{'\t'});
			string[] areas = fields[0].Split(new char[]{';'}).Trim();

			foreach(string area in areas)
			{
				TierCategoryFeedback tcfb = null;
				List<TierCategoryFeedback> tcfbList = null;

				if(area == Clothing.Tier.INFORMAL.ToString())
					tcfbList = informalFB;
				else if(area == Clothing.Tier.CASUAL.ToString())
					tcfbList = casualFB;
				else if(area == Clothing.Tier.BUSINESS_CASUAL.ToString())
					tcfbList = busCasFB;
				else if(area == Clothing.Tier.BUSINESS_PROFESSIONAL.ToString())
					tcfbList = busProfFB;
				
				if(tcfbList == null)
					continue;

				string[] interviewTiers = fields[1].Split(new char[]{';'}).Trim();
				string clothingCategory = fields[2].Trim();
				tcfb = tcfbList.FirstOrDefault(search => search.gender == (male? CrossSceneInfo.GenderEnum.MALE : CrossSceneInfo.GenderEnum.FEMALE)
				                               && interviewTiers.ContainsAll(Clothing.Tier.GetNames(search.tiers.ToArray()))
				                               && clothingCategory == Clothing.Category.GetName(search.category));
				if(tcfb == null){
					tcfbList.Add(new TierCategoryFeedback());
					tcfb = tcfbList.Last();
					tcfb.gender = (male? CrossSceneInfo.GenderEnum.MALE : CrossSceneInfo.GenderEnum.FEMALE);
					foreach(string tier in interviewTiers)
						tcfb.tiers.Add(Clothing.Tier.GetEnum(tier));
					tcfb.category = Clothing.Category.GetEnum(clothingCategory);
				}

				if(fields.Length >= 4)
					tcfb.feedback = fields[3];
			}
		}
	}

	public void ClearFeedback()
	{
		informalFB = new List<TierCategoryFeedback>();
		casualFB = new List<TierCategoryFeedback>();
		busCasFB = new List<TierCategoryFeedback>();
		busProfFB = new List<TierCategoryFeedback>();
	}
	#endif
	#endregion 

	/// Extra ///

	#region SubClasses
	[System.Serializable]
	public class ClothingSetup
	{
		public List<Clothing.Info> clothing = new List<Clothing.Info>();
		public Renderer[] renderers = new Renderer[0];
	}


	[System.Serializable]
	public class OtherSetup
	{
		public string category;
		public List<Clothing.Info> clothing = new List<Clothing.Info>();
		public Renderer renderer;
		public Renderer otherRenderer;
	}

	[System.Serializable]
	public class TierCategoryFeedback
	{
		public CrossSceneInfo.GenderEnum gender;
		public List<Clothing.Tier.TierEnum> tiers = new List<Clothing.Tier.TierEnum>();
		public Clothing.Category.CategoryEnum category;
		public string feedback;
	}

	[System.Serializable]
	public class BodyPartRender
	{
		public Renderer renderer;
		public Material material;
	}
	#endregion 
}
#pragma warning restore 0649

