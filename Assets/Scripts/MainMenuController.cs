using Simcoach.SkillArcade;
using UnityEngine;
//using Simcoach.SkillArcade;

public class MainMenuController : MonoBehaviour
{
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
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
