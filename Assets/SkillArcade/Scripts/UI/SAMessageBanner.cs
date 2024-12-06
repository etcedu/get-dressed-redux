using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Simcoach.SkillArcade.UI
{
    public class SAMessageBanner : MonoBehaviour
    {

        public SimplePanel thisPanel;

        public Text mainTextLabel;
        public Text subTextLabel;
        public SSALocalizedText main;
        public SSALocalizedText sub;

        public float showTime;

        public void ShowMessage(string mainTextKey, string subTextKey, bool autoHide = true, bool useKeyForSub = true)
        {
            main.SetKey(mainTextKey);

            if (!useKeyForSub)
            {
                sub.suppress = true;
                subTextLabel.text = subTextKey;
            }
            else
            {
                sub.suppress = false;
                sub.SetKey(subTextKey);
            }
            
            thisPanel.Show();
            if (autoHide)
                StartCoroutine(hideTimer());
        }

        public void Hide()
        {
            thisPanel.Hide();
        }

        IEnumerator hideTimer()
        {
            yield return new WaitForSeconds(showTime);
            Hide();
        }
    }
}