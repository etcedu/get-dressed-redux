using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTexture : MonoBehaviour
{
    [SerializeField] Texture2D maleTexture;
    [SerializeField] Texture2D femaleTexture;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SkinnedMeshRenderer>().sharedMaterial.mainTexture = GlobalData.currentCharacterSelection.gender == Gender.MALE ? maleTexture : femaleTexture;   
    }
}
