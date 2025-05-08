using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public void StartButton_OnClick()
    {
        SceneLoader.LoadScene("LevelSelection");
    }

    private void Start()
    {
        /*
        EventManagerBase.ReadyToShowBadges();
        SkillArcadeUIManager.instance.ShowMainButton();
        
        DynamicAdManager.instance.ShowMainMenuAds(2);
        DynamicAdManager.instance.SearchForAds();
        DynamicAdManager.instance.UpdateAdData();
        */
    }

    public void SendSilverBadge()
    {
        /*
        EventManagerBase.GiveSilverBadge();
        if (Simcoach.Net.WebComms.LoggedIn())
        {
            EventManagerBase.SendBadgeEvent();
        }
        */
    }

    public void SendGoldBadge()
    {
        /*
        EventManagerBase.GiveGoldBadge();
        if (Simcoach.Net.WebComms.LoggedIn())
        {
            EventManagerBase.SendBadgeEvent();
        }
        */
    }
}
