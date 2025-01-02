using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
	[SerializeField] List<Toggle> colorToggles = new List<Toggle>();
	
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
	CrossSceneInfo.GenderEnum _currentGender;
	Color _currentColor;
	int _currentColorIndex;
	
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
		foreach(Toggle colorToggle in colorToggles)
			ColorSetup(colorToggle);
	}
	
	
	// Establish starting category
	void Start()
	{
		if (!CrossSceneInfo.CharacterColorChosen)
		{
			colorToggles[0].SetIsOnWithoutNotify(true);
			SetColor(true);
		}
		else
		{ 
			colorToggles[CrossSceneInfo.CharacterColorToggleIndex].SetIsOnWithoutNotify(true);
			SetColor(true);
		}

		SetGender(CrossSceneInfo.Gender);
	}
	
	/// <summary>
	/// Sets the gender.
	/// </summary>
	/// <param name="string">"male" is the only one recognized, otherwise default to female</param>
	public void SetGender(string gender)
	{
        SetGender(gender == "male" ? CrossSceneInfo.GenderEnum.MALE : CrossSceneInfo.GenderEnum.FEMALE);
    }

	void SetGender(CrossSceneInfo.GenderEnum gender)
	{
		_currentGender = gender;

        if (_currentGender == CrossSceneInfo.GenderEnum.MALE)
        {
            foreach (BodyPartRender bpr in maleSetup)
                bpr.renderer.sharedMaterial = bpr.material;
            characterAnimator.runtimeAnimatorController = maleController;
        }
        else
        {
            foreach (BodyPartRender bpr in femaleSetup)
                bpr.renderer.sharedMaterial = bpr.material;
            characterAnimator.runtimeAnimatorController = femaleController;
        }
    }
		
	void ColorSetup(Toggle toggle)
	{
		toggle.onValueChanged.AddListener((x) => { SetColor(x); });
	}
	
	void SetColor(bool isOn)
	{
		for (int i = 0; i < colorToggles.Count; i++)
		{
			if (colorToggles[i].isOn)
			{
				_currentColor = colorToggles[i].transform.GetChild(0).GetComponent<Image>().color;
				_currentColorIndex = i;
			}
		}

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
		CrossSceneInfo.CharacterColorToggleIndex = _currentColorIndex;
		CrossSceneInfo.CharacterColorHex = _currentColor.ToHexStringRGBA();
		CrossSceneInfo.Gender = _currentGender;
		Quit();
	}

	public void Quit()
	{
        SceneManager.LoadScene("Map");
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("Start Screen");
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

