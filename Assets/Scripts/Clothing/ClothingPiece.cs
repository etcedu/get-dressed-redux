using Clothing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingPiece
{
    // Identifier
    public string Tag { get; private set; }
    public string DisplayTag { get; private set; }
        //for loading from Resources
    public string ImageAssetName { get; private set; }
    public Gender GenderRole { get; private set; }
    public Category Category { get; private set; }
    public Tier[] Tiers { get; private set; }
}
