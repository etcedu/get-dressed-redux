using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothingPieceSelectionToggle : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Image shadowIcon;
    public ClothingPiece clothingPiece;
    public Toggle toggle;

    public void InitButton(ClothingPiece _clothingPiece)
    {
        clothingPiece = _clothingPiece;
        icon.sprite = clothingPiece.Connection.icon;
        shadowIcon.sprite = clothingPiece.Connection.icon;
    }

    public void SpeakName()
    {
        SimpleRTVoiceExample.Instance.Speak("default", clothingPiece.DisplayName);
        FindObjectOfType<ClothingNameTextObject>().Show(clothingPiece.DisplayName, gameObject);
    }
}
