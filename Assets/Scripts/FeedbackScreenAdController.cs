using Simcoach.SkillArcade;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackScreenAdController : MonoBehaviour
{
    private TweenAlpha resourcesPanel;

    private void Awake()
    {
        resourcesPanel = GetComponent<TweenAlpha>();
    }

    private void Start()
    {
        TurnOffPanel();
        
        /*if (DynamicAdManager.instance != null)
        {
            DynamicAdManager.instance.SearchForAds();
            DynamicAdManager.instance.UpdateAdData();
        }
        */
    }

    public void TurnOnPanel()
    {
        resourcesPanel.PlayForward();
    }

    public void TurnOffPanel()
    {
        resourcesPanel.PlayReverse();
    }

    public void SendNumberOfPlaysData()
    {
        List<Simcoach.SkillArcade.Pair<string, string>> gameData = new List<Simcoach.SkillArcade.Pair<string, string>>();
        gameData.Add(new Simcoach.SkillArcade.Pair<string, string>("completed", true.ToString()));
        //EventManagerBase.SendDataEvent("levelCompleted", gameData);
    }
}
