using System.Collections;
using System.Collections.Generic;
using Simcoach.SimpleJSON;
using UnityEngine;
using UnityEngine.UI;

namespace Simcoach.SkillArcade {

    public class DynamicPopupAd : DynamicAd {

        public RawImage image;
        public OpenURL openUrl;
        public GameObject windowCloser;
        public bool IsVisible { get { return GetComponent<UI.SimplePanel>().showing; } }

        override public void Start()
        {
            base.Start();
            windowCloser.SetActive(false);
        }

        public void Update()
        {
            if (GetComponent<UI.SimplePanel>().showing && SkillArcadeUIManager.instance.badgePopup.GetComponent<UI.SimplePanel>().showing)
                windowCloser.SetActive(true);
        }

        public override void SetToDefaults()
        {
            base.SetToDefaults();
            image.texture = defaultMainImage;
        }

        public override void ProcessReturnData(JSONNode data)
        {
            base.ProcessReturnData(data);
        }

        public override void SetMainTexture(Texture texture)
        {
            base.SetMainTexture(texture);
            image.texture = loadedMainImage;
        }

        public override void SetSecondaryTexture(Texture texture)
        {
            base.SetSecondaryTexture(texture);
        }
        
        public void ShowAd()
        {
            GetComponent<UI.SimplePanel>().Show();
        }

        public void HideAd()
        {
            GetComponent<UI.SimplePanel>().Hide();
            if (!SkillArcadeUIManager.instance.badgePopup.GetComponent<UI.SimplePanel>().showing &&
                !SkillArcadeUIManager.instance.mainMenuPanel.showing &&
                !SkillArcadeUIManager.instance.signInPanel.showing)
            {
                SkillArcadeUIManager.instance.overlayFade.Undarken();
            }

            windowCloser.SetActive(false);
        }

        public void Visit_OnClick()
        {
            openUrl.pageTitle = loadedText;
            openUrl.URL = loadedSponsorURL;
            openUrl.Launch();
        }
    }
}