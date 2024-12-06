using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simcoach.SkillArcade
{
    public class SkillArcadeLayerAd : DynamicAd
    {
        UI.SimplePanel panel;
        public RawImage image;

        public Image arrowImage;
        public TweenAlpha_SA notificationIcon;

        override public void Start()
        {
            base.Start();
            if (defaultMainImage == null)
                HideAd();

            panel = GetComponent<UI.SimplePanel>();
        }

        public override void SetToDefaults()
        {
            base.SetToDefaults();
            image.texture = defaultMainImage;
        }
        
        public override void SetMainTexture(Texture texture)
        {
            base.SetMainTexture(texture);
            image.texture = texture;
        }

        public void TrayButton_OnTap()
        {
            if (panel.showing)
                CloseAd();
            else
                OpenAd();
        }

        public void OpenAd()
        {
            if (loadedMainImage == null) return;

            gameObject.SetActive(true);
            arrowImage.transform.localScale = new Vector3(1, 1, 1);
            notificationIcon.PlayReverse_FromBeginning();
            panel.Show();
        }

        public void CloseAd()
        {
            arrowImage.transform.localScale = new Vector3(-1, 1, 1);
            notificationIcon.PlayForward();
            panel.Hide();
        }

        public void HideAd()
        {
            gameObject.SetActive(false);
        }
                
        public void OnTap()
        {
            Application.OpenURL(loadedSponsorURL);
        }
    }
}