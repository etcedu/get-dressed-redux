using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 0649
[System.Serializable]
public class AvatarManager : MonoBehaviour
{
	/// Variables ///
	
	#region Serialized Fields
	[SerializeField]
	private Animator characterAnimator;
	[SerializeField]
	private RuntimeAnimatorController maleController;
	[SerializeField]
	private RuntimeAnimatorController femaleController;

	[SerializeField]
	private ButtonStateToggle maleToggle;
	[SerializeField]
	private ButtonStateToggle femaleToggle;
	[SerializeField]
	private List<ButtonStateToggle> colorButtons = new List<ButtonStateToggle>();
	
	public BodyPartRender[] maleSetup = new BodyPartRender[0];
	public BodyPartRender[] femaleSetup = new BodyPartRender[0];

#if UNITY_EDITOR
	[ContextMenu ("Renderers - Male To Female")]
	void RenderersMaleToFemale()
	{
		for(int i = 0; i < maleSetup.Count(); i++)
		{
			if(i >= femaleSetup.Count())
				ArrayUtility.Add(ref femaleSetup, new BodyPartRender(maleSetup[i].renderer));
			else
				femaleSetup[i].renderer = maleSetup[i].renderer;
		}
	}
#endif
	#endregion
	
	#region Unserialized Fields
	private CrossSceneInfo.GenderEnum _currentGender;
	private GameObject _currentGenderGO;
	private Color _currentColor;
	private GameObject _currentColorGO;
	
	// Gameobject/Color ties
	[System.NonSerialized]
	private Dictionary<GameObject, CrossSceneInfo.GenderEnum> _goGender = new Dictionary<GameObject, CrossSceneInfo.GenderEnum>();
	[System.NonSerialized]
	private Dictionary<CrossSceneInfo.GenderEnum, GameObject> _genderGO = new Dictionary<CrossSceneInfo.GenderEnum, GameObject>();
	[System.NonSerialized]
	private Dictionary<GameObject, Color> _goColor = new Dictionary<GameObject, Color>();
	[System.NonSerialized]
	private Dictionary<string, GameObject> _colorGO = new Dictionary<string, GameObject>();
	#endregion
	
	/// Functions ///
	
	#region Setup
	// Populate the grids and set up interactions
	void Awake()
	{
		GenderSetup(maleToggle, CrossSceneInfo.GenderEnum.MALE);
		GenderSetup(femaleToggle, CrossSceneInfo.GenderEnum.FEMALE);

		foreach(ButtonStateToggle colorButton in colorButtons)
			ColorSetup(colorButton);
	}
	
	
	// Establish starting category
	void Start()
	{
		if(!CrossSceneInfo.CharacterColorChosen)
			SetColor(colorButtons[0].gameObject);
		else
		{
			SetColor(_colorGO[CrossSceneInfo.CharacterColorHex]);
		}

		SetGender(_genderGO[CrossSceneInfo.Gender]);
	}
	
	
	// Setup the toggle, direct the interaction, and include in the appropriate dictionaries
	void GenderSetup(ButtonStateToggle bst, CrossSceneInfo.GenderEnum gender)
	{
		bst.gameObject.AddMissingComponent<ForwardTouch>().Clicked += SetGender;
		_goGender.Add(bst.gameObject, gender);
		_genderGO.Add(gender, bst.gameObject);
	}

	/// <summary>
	/// Sets the gender.
	/// </summary>
	/// <param name="bst">Bst.</param>
	void SetGender(GameObject bst)
	{
		if(_currentGenderGO != null)
		{
			if(_currentGenderGO == bst) return;
			_currentGenderGO.GetComponent<ButtonStateToggle>().SetToStart();
		}
		_currentGenderGO = bst;
		bst.GetComponent<ButtonStateToggle>().SetToToggle();
		_currentGender = _goGender[bst];
		//CrossSceneInfo.Gender = _goGender[bst];
		
		if(_currentGender == CrossSceneInfo.GenderEnum.MALE)
		{
			foreach(BodyPartRender bpr in maleSetup)
				bpr.renderer.sharedMaterial = bpr.material;
			characterAnimator.runtimeAnimatorController = maleController;
		}
		else
		{
			foreach(BodyPartRender bpr in femaleSetup)
				bpr.renderer.sharedMaterial = bpr.material;
			characterAnimator.runtimeAnimatorController = femaleController;
		}
	}

	
	void ColorSetup(ButtonStateToggle bst)
	{
		bst.gameObject.AddMissingComponent<ForwardTouch>().Clicked += SetColor;
		_goColor.Add(bst.gameObject, bst._startingState.colorNormal);
		//Debug.Log(bst._startingState.colorNormal.ToHexStringRGBA());
		_colorGO.Add(bst._startingState.colorNormal.ToHexStringRGBA(), bst.gameObject);
	}
	
	void SetColor(GameObject bst)
	{
		if(_currentColorGO != null)
		{
			if(_currentColorGO == bst) return;
			_currentColorGO.GetComponent<ButtonStateToggle>().SetToStart();
		}
		_currentColorGO = bst;
		bst.GetComponent<ButtonStateToggle>().SetToToggle();
		_currentColor = _goColor[bst];
		//CrossSceneInfo.CharacterColor = _goColor[bst];
		
		foreach(BodyPartRender bpr in maleSetup)
			if(bpr.material != null)
				bpr.material.color = _currentColor;
		
		foreach(BodyPartRender bpr in femaleSetup)
			if(bpr.material != null)
				bpr.material.color = _currentColor;
	}
	#endregion 

	public void SaveAndQuit()
	{
		CrossSceneInfo.CharacterColorHex = _currentColor.ToHexStringRGBA();
		CrossSceneInfo.Gender = _currentGender;
		Quit();
	}

	public void Quit()
	{
        SceneManager.LoadScene("Map");
		//Application.LoadLevel("Map");
	}

	
	[System.Serializable]
	public class BodyPartRender
	{
		public BodyPartRender(){}
		public BodyPartRender(Renderer renderer)
		{
			this.renderer = renderer;
		}
		public BodyPartRender(Material material)
		{
			this.material = material;
		}
		public BodyPartRender(Renderer renderer, Material material)
		{
			this.renderer = renderer;
			this.material = material;
		}
		public Renderer renderer;
		public Material material;
	}
}
#pragma warning restore 0649

