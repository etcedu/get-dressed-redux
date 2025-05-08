using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] FancyScrollView.TheFitCharacterSelect.ScrollView scrollView;
        
    public void SelectCharacter()
    {
        GlobalData.SetCharacter(scrollView.GetCurrentCharacter().characterTag);

        SceneManager.LoadScene("Dressing");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
