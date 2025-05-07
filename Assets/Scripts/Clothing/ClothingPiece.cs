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
}
