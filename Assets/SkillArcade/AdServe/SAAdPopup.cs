using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simcoach.SkillArcade
{
    public class SAAdPopup : MonoBehaviour
    {
        public UI.SimplePanel popupPanel;
        public RawImage zoomImage;
        public Text descLabel;
        public OpenURL openURL;

        public void Set(Texture2D texture, string text, string _url)
        {
            zoomImage.texture = texture;
            if (descLabel != null)
                descLabel.text = text;
            openURL.URL = _url;
        }

        public void ShowAd(Texture2D texture, string text, string _url)
        {
            Set(texture, text, _url);
            popupPanel.Show();
            SkillArcadeUIManager.instance.overlayFade.Darken();
        }

        public void Hide()
        {
            popupPanel.Hide();
        }

        public void VisitLink_OnClick()
        {
            openURL.Launch();
        }
    }
}