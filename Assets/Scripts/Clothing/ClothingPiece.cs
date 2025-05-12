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
    public List<Tier> Tiers;
    public ClothingModelConnection Connection;


    public string FeedbackName;
    public string GoodFeedback;
    public string OKFeedback;
    public string BadFeedback;

    //set by character at runtime
    public Score scoreForCurrentCharacter;

    public string GetFeedback()
    {
        if (scoreForCurrentCharacter == Score.GOOD) return GoodFeedback;
        if (scoreForCurrentCharacter == Score.OK) return OKFeedback;
        if (scoreForCurrentCharacter == Score.BAD) return BadFeedback;
        else return "NoScore";
    }

}
