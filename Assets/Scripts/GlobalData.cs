using System;
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
    HEAD, TOP, BOTTOM, FEET, OTHER
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
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.headPieces);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.topPieces);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.bottomPieces);
        Instance.SetScoresForCurrentCharacterClothingPieces(currentCharacterSelection.feetPieces);
    }

    void SetScoresForCurrentCharacterClothingPieces(List<ClothingPiece> clothingSet)
    {
        if (clothingSet.Count == 1)
        {
            clothingSet[0].scoreForCurrentCharacter = Score.GOOD;
        }
        if (clothingSet.Count == 2)
        {
            clothingSet[0].scoreForCurrentCharacter = Score.GOOD;
            clothingSet[1].scoreForCurrentCharacter = Score.BAD;
        }
        else if (clothingSet.Count == 3)
        {
            clothingSet[0].scoreForCurrentCharacter = Score.GOOD;
            clothingSet[1].scoreForCurrentCharacter = Score.OK;
            clothingSet[2].scoreForCurrentCharacter = Score.BAD;
        }
    }

    public static IList<CharacterData> GetCharacters()
    {
        return Instance.theCharacterRoster.characters;
    }

    public static int GetScoreForPiece(ClothingPiece clothingPiece)
    {
        return Enum.GetValues(typeof(Score)).Length - (int)clothingPiece.scoreForCurrentCharacter;
    }

    public static float GetOverallScore()
    {
        float totalScore = 0;

        totalScore += GetScoreForPiece(selectedHeadPiece);
        totalScore += GetScoreForPiece(selectedTopPiece);
        totalScore += GetScoreForPiece(selectedBottomPiece);
        totalScore += GetScoreForPiece(selectedFeetPiece);

        return (totalScore / 12f);
    }

    public static List<ClothingPiece> GetListOfSelectedClothes()
    {
        List<ClothingPiece> list = new List<ClothingPiece>
        {
            selectedHeadPiece,
            selectedTopPiece,
            selectedBottomPiece,
            selectedFeetPiece
        };
        return list;
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
}