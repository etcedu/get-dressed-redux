using System.Collections.Generic;
using UnityEngine;
using static CrossSceneInfo;

[System.Serializable]
public enum Gender
{
    EITHER, MALE, FEMALE
}

[System.Serializable]
public enum Category
{
    HEAD, TOP, BOTTOM, FEET
}

[System.Serializable]
public enum Tier
{
    INFORMAL, CASUAL, BUSINESS_CASUAL, BUSINESS_PROFESSIONAL
}


public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance { get; private set; }

    public static CharacterData currentCharacterSelection;
    public static ClothingPiece selectedHeadPiece;
    public static ClothingPiece selectedTopPiece;
    public static ClothingPiece selectedBottomPiece;
    public static ClothingPiece selectedFeetPiece;

    [SerializeField] List<CharacterData> characters = new List<CharacterData>();
    [SerializeField] ClothingCloset theCloset;

    public void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        currentCharacterSelection = characters[0];
        InitNewLevel();
    }

    public void InitNewLevel()
    {
        selectedHeadPiece = null;
        selectedTopPiece = null;
        selectedBottomPiece = null;
        selectedFeetPiece = null;
    }

    public static void SetCharacter(CharacterData charData)
    {
        currentCharacterSelection = charData;
    }

    public static IList<CharacterData> GetCharacters()
    {
        return Instance.characters;
    }

    public static ClothingPiece GetPieceOfClothing(string name)
    {
        return Instance.theCloset.GetClothingPiece(name);
    }

    public static void SetClothingSelection(ClothingPiece clothingPiece)
    {
        switch (clothingPiece.Category)
        {
            case Category.HEAD:
                selectedHeadPiece = clothingPiece;
                break;
            case Category.TOP:
                selectedTopPiece = clothingPiece;
                break;
            case Category.BOTTOM:
                selectedBottomPiece = clothingPiece;
                break;
            case Category.FEET:
                selectedFeetPiece = clothingPiece;
                break;
        }
    }

    #region Gender
    /// <summary>
    /// Gets the current gender, loading it from save if necessary
    /// </summary>
    public static GenderEnum Gender
    {
        get
        {
            if (!_genderLoaded)
                _gender = (GameBase.Strings.GetValue("Gender", "Male") == "Male") ? GenderEnum.MALE : GenderEnum.FEMALE;
            return _gender;
        }
        set
        {
            _gender = value;
            GameBase.Strings.SetValue("Gender", (value == GenderEnum.MALE) ? "Male" : "Female");
            GameBase.Strings.Save();
        }
    }
    /// <summary>
    /// DO NOT REFERENCE DIRECTLY. Use Gender instead.
    /// </summary>
    private static GenderEnum _gender;
    /// <summary>
    /// Flag for loading gender
    /// </summary>
    private static bool _genderLoaded = false;
    #endregion

    private const int interviewPassedCutoff = 400;
    public static int PassingCuttoff
    {
        get { return interviewPassedCutoff; }
    }
}