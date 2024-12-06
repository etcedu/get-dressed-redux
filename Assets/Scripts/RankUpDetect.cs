using UnityEngine;
using System.Collections.Generic;
using System;
using Simcoach.Net;
using Simcoach.SkillArcade;

public class RankUpDetect : MonoBehaviour
{
	public List<EventDelegate> rankUpEvents = new List<EventDelegate>();

    private void Awake()
    {
        /*
        if (DynamicAdManager.instance != null)
        {
            DynamicAdManager.instance.SearchForAds();
            DynamicAdManager.instance.UpdateAdData();
        }
        */
    }

    // Use this for initialization
    void Start ()
	{
        //ShowFullScreenAd();
        //CheckAndSendBadgeEvents();

		if(CrossSceneInfo.RankChanged)
		{
            //CheckAndSendBadgeEvents();

			Debug.Log("rank changed");
			CrossSceneInfo.RankChangeReset();
			EventDelegate.Execute(rankUpEvents);

            /*
			var dataEvent = new SkillEvent();
			dataEvent.AddString("EventType", "RankChange");
			dataEvent.AddString("NewRank", CrossSceneInfo.RankName);
			dataEvent.Save();
		*/

            EventRecorder.RecordRankChangedEvent(CrossSceneInfo.RankName);
        }
	}

    public void ShowFullScreenAd()
    {
        if (!GlobalData.sawAdThisGame)
        {
            if (DynamicAdManager.instance != null)
            {
                DynamicAdManager.instance.ShowFullScreenAd("FullScreenAd_A");
            }
            GlobalData.sawAdThisGame = true;
        }
    }

    private void CheckAndSendBadgeEvents()
    {
        if(WebComms.LoggedIn())
        {
            if (CrossSceneInfo.RankName == "Entry Level")
                EventManagerBase.GiveBronzeBadge();

            if (CrossSceneInfo.RankName == "Pro")
                EventManagerBase.GiveSilverBadge();

            if (CrossSceneInfo.RankName == "JobPro")
                EventManagerBase.GiveGoldBadge();

            EventManagerBase.SendBadgeEvent();
            EventManagerBase.ReadyToShowBadges();
        }
        else
        {
            if(CrossSceneInfo.RankName == "Entry Level")
            {
                EventManagerBase.GiveBronzeBadge();
                SkillArcadeUIManager.instance.ShowOfflineBronzeBadge();
            }
        }
    }
}

