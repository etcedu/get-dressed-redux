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

public enum Score
{
    GOOD, OK, BAD
}

public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance { get; private set; }

    public static CharacterData currentCharacterSelection;
    public static ClothingPiece selectedHeadPiece;
    public static ClothingPiece selectedTopPiece;
    public static ClothingPiece selectedBottomPiece;
    public static ClothingPiece selectedFeetPiece;

    [SerializeField] ClothingCloset theCloset;
    [SerializeField] CharacterRoster theCharacterRoster;

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

    }

    public static void InitNewLevel()
    {
        selectedHeadPiece = null;
        selectedTopPiece = null;
        selectedBottomPiece = null;
        selectedFeetPiece = null;
    }

    public static void SetCharacter(string characterTag)
    {
        currentCharacterSelection = Instance.theCharacterRoster.GetCharacter(characterTag);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.headOptions);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.topOptions);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.bottomOptions);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.feetOptions);
    }

    void SetScoresForCurrentCharacterClothingPieces(string[] clothingSet)
    {
        if (clothingSet.Length == 1)
        {
            GetPieceOfClothing(clothingSet[0]).scoreForCurrentCharacter = Score.GOOD;
        }
        if (clothingSet.Length == 2)
        {
            GetPieceOfClothing(clothingSet[0]).scoreForCurrentCharacter = Score.GOOD;
            GetPieceOfClothing(clothingSet[1]).scoreForCurrentCharacter = Score.BAD;
        }
        else if (clothingSet.Length == 3)
        {
            GetPieceOfClothing(clothingSet[0]).scoreForCurrentCharacter = Score.GOOD;
            GetPieceOfClothing(clothingSet[1]).scoreForCurrentCharacter = Score.OK;
            GetPieceOfClothing(clothingSet[2]).scoreForCurrentCharacter = Score.BAD;
        }
    }

    public static IList<CharacterData> GetCharacters()
    {
        return Instance.theCharacterRoster.characters;
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

    public static Color CharacterColor
    {
        get
        {
            Color32 retColor = DefaultCharacterColor;
            ColorExtensions.TryParseHexStringRGBA(GameBase.Strings.GetValue("CharacterColor", DefaultCharacterColor.ToHexStringRGBA()),
                                                  out retColor);
            return (Color)retColor;
        }
    }

    private const int interviewPassedCutoff = 400;
    public static int PassingCuttoff
    {
        get { return interviewPassedCutoff; }
    }
}