using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simcoach.SimpleJSON;
using Simcoach.Net;

namespace Simcoach.SkillArcade
{

    public class DynamicAd : MonoBehaviour
    {

        public Texture2D defaultMainImage;
        public Texture2D defaultSecondaryImage;
        public string defaultText;
        public string defaultSponsorURL;

        public Texture2D loadedMainImage;
        public Texture2D loadedSecondaryImage;
        public string loadedText;
        public string loadedSponsorURL;

        public string AdID;
        public JSONNode data;

        virtual public void Start()
        {
            SetToDefaults();
        }

        virtual public void LogIn()
        {

        }

        virtual public void LogOut()
        {
            SetToDefaults();
        }

        virtual public void SetToDefaults()
        {
            loadedMainImage = defaultMainImage;
            loadedSecondaryImage = defaultSecondaryImage;
            loadedText = defaultText;
            loadedSponsorURL = defaultSponsorURL;
        }

        virtual public void ProcessReturnData(JSONNode data)
        {
            Debug.Log("processing: " + AdID);

            loadedText = defaultText;
            loadedText = data["Text"];
            if (string.IsNullOrEmpty(loadedText) || loadedText == " ")
                loadedText = defaultText;

            loadedSponsorURL = defaultSponsorURL;
            loadedSponsorURL = data["SponsorURL"];
            if (string.IsNullOrEmpty(loadedSponsorURL) || loadedSponsorURL == " ")
                loadedSponsorURL = defaultSponsorURL;

            if (string.IsNullOrEmpty(data["URL"]) || data["URL"] == " ")
                SetMainTexture(defaultMainImage);
            else
                DynamicAdManager.instance.DownloadTexture(data["URL"], (Texture texture) => { SetMainTexture(texture); });

            if (string.IsNullOrEmpty(data["URL2"]) || data["URL2"] == " ")
                SetSecondaryTexture(defaultSecondaryImage);
            else
                DynamicAdManager.instance.DownloadTexture(data["URL2"], (Texture texture) => { SetSecondaryTexture(texture); });
        }

        virtual public void SetMainTexture(Texture texture)
        {
            loadedMainImage = (Texture2D)texture;
        }

        virtual public void SetSecondaryTexture(Texture texture)
        {
            loadedSecondaryImage = (Texture2D)texture;
        }
        
        public void OnInteract()
        {
            DynamicAdManager.instance.ShowAdWindow(this);
        }
    }
}