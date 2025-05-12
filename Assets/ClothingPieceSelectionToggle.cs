using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothingPieceSelectionToggle : MonoBehaviour
{
    [SerializeField] Image icon;
    public ClothingPiece clothingPiece;

    public void InitButton(ClothingPiece _clothingPiece)
    {
        clothingPiece = _clothingPiece;
        icon.sprite = clothingPiece.Connection.icon;
    }
}
