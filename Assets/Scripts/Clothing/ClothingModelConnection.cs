using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ClothingModelConnection", fileName = "New Clothing Model Connection")]
[System.Serializable]
public class ClothingModelConnection : ScriptableObject
{
    public Sprite icon;
    public List<Material> maleMaterials;
    public List<Material> femaleMaterials;
    public List<Texture> textures;
}
