using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    int unlockClickCounter = 0;
    [SerializeField] GameObject unlockedText;

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public void StartButton_OnClick()
    {
        SceneLoader.LoadScene("LevelSelection");
    }

    public void UnlockOnClick()
    {
        unlockClickCounter++;
        if (unlockClickCounter > 3)
        {
            GlobalData.SetTutorialState(true);
            unlockedText.SetActive(true);
        }
    }
}
