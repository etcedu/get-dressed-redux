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
    HEAD, TOP, BOTTOM, FEET, OTHER, DRESS
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

    public static bool isTutorial;
    public static int lastCharacterIndex;

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

        isTutorial = currentCharacterSelection.characterTag.ToLower().Contains("tutorial");
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

    public static int GetTotalNumericalScore()
    {
        int totalScore = 0;

        totalScore += GetScoreForPiece(selectedHeadPiece);
        totalScore += GetScoreForPiece(selectedTopPiece);
        totalScore += GetScoreForPiece(selectedBottomPiece);
        totalScore += GetScoreForPiece(selectedFeetPiece);

        return totalScore;
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
            case Category.DRESS:
                selectedTopPiece = clothingPiece;
                selectedBottomPiece = clothingPiece;
                break;
        }
    }

    public static bool GetTutorialFinished()
    {
        return PlayerPrefs.GetInt("FinishedTutorial", 0) == 1;
    }

    public static void SetTutorialState(bool finished)
    {
        PlayerPrefs.SetInt("FinishedTutorial", finished ? 1 : 0);
    }

    public static bool GetCharacterCompleted(string charTag)
    {
        return PlayerPrefs.GetInt($"Completed_{charTag}", 0) == 1;
    }

    public static void SetCharacterCompleted(string charTag, bool completed)
    {
        if (GetCharacterCompleted(charTag))
            return;

        PlayerPrefs.SetInt($"Completed_{charTag}", completed ? 1 : 0);
    }

    public static int GetLastCharacterIndex()
    {
        Debug.Log($"GetLastCharacterIndex: {PlayerPrefs.GetInt($"lastCharacterIndex", 0)}");
        return PlayerPrefs.GetInt($"lastCharacterIndex", 0);
    }

    public static void SetLastCharacterIndex(int index)
    {
        PlayerPrefs.SetInt($"lastCharacterIndex", index);
    }
}