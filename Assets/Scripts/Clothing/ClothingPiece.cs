using Clothing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingPiece
{
    // Identifier
    public string Tag { get; set; }
    public string DisplayName { get; set; }
        //for loading from Resources
    public string ImageAssetName { get; set; }
    public Gender GenderRole { get; set; }
    public Category Category { get; set; }
    public List<Tier> Tiers { get; set; }

    // Renderer sprites
    public Texture[] Pieces { get; set; }
 
}
