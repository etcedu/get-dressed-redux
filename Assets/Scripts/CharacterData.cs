using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CharacterDataSO", fileName = "New Character Data")]
[System.Serializable]
public class CharacterDataSO : ScriptableObject
{
    public string characterTag;
    public string characterName;
    public string jobTitle;
    public string description;
    public string jobAttireDescription;
    public string imageAssetPath;
    public string[] headOptions;
    public string[] topOptions;
    public string[] bottomOptions;
    public string[] feetOptions;
    public string[] otherClothes;
}

[System.Serializable]
public class CharacterData
{
    public string characterTag;
    public string characterName;
    public Gender gender;
    public string jobTitle;
    public string description;
    public string jobAttireDescription;
    public string imageAssetPath;
    public string[] headOptions;
    public string[] topOptions;
    public string[] bottomOptions;
    public string[] feetOptions;
    public string[] otherClothes;
    public Color skinColor;
    public string winFeedback;
    public string loseFeedback;

    //combined data test
    public List<ClothingPiece> headPieces = new();
    public List<ClothingPiece> topPieces = new();
    public List<ClothingPiece> bottomPieces = new();
    public List<ClothingPiece> feetPieces = new();
}