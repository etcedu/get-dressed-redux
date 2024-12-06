using Simcoach.SkillArcade;
using UnityEngine;

public class NotInMainSceneController : MonoBehaviour
{
    private void Start()
    {
        if (SkillArcadeUIManager.instance != null)
        {
            //SkillArcadeUIManager.instance.HideMainButton();
            //DynamicAdManager.instance.HideMainMenuAds();
        }
    }
}
