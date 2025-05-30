using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string characterTag;
    public string characterName;
    public Gender gender;
    public string jobTitle;
    public string description;
    public string imageAssetPath;
    public Color skinColor;
    public string winFeedback;
    public string okFeedback;
    public string loseFeedback;

    //combined data test
    public List<ClothingPiece> headPieces = new();
    public List<ClothingPiece> topPieces = new();
    public List<ClothingPiece> bottomPieces = new();
    public List<ClothingPiece> feetPieces = new();
    public List<ClothingPiece> otherPieces = new();
}