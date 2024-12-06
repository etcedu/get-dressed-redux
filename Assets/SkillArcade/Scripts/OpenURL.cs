using SimcoachGames.EventRecorder;
using UnityEngine;

namespace Simcoach.SkillArcade
{
    public class OpenURL : MonoBehaviour
    {

        public string URL;
        public string pageTitle;

        public void Launch()
        {
            if (SimcoachGames.EventRecorder.Login.SimcoachLoginUserManagement.Instance.UseParentalGate)
            {
                ParentalGateManager.RequestParentalGate((result) =>
                {
                    if (result)
                        OpenBrowser();
                });
            }
            else
            {
                OpenBrowser();
            }
        }

        void OpenBrowser()
        {
            UniWebViewSafeBrowsing safeBrowser = UniWebViewSafeBrowsing.Create(URL);
            safeBrowser.Show();
        }
    }

}