using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] FancyScrollView.TheFitCharacterSelect.ScrollView scrollView;
        
    public void SelectCharacter()
    {
        GlobalData.SetCharacter(scrollView.GetCurrentCharacter());
    }
}
