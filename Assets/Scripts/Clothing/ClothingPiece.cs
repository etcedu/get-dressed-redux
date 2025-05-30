using Clothing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClothingPiece
{
    public string Tag;
    public string DisplayName;
    public Gender GenderRole;
    public Category Category;
    public ClothingModelConnection Connection;

    public string FeedbackName;
    public string Feedback;

    //set by character at runtime
    public Score scoreForCurrentCharacter;
}
