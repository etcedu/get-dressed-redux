using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simcoach.SimpleJSON;

namespace Simcoach.SkillArcade
{
    public class DynamicAdManager : MonoBehaviour
    {
        public static DynamicAdManager instance;

        public List<SkillArcadeLayerAd> mainMenuAds;
        public List<DynamicPopupAd> fullscreenWindows;

        List<DynamicAd> ads = new List<DynamicAd>();
        List<string> adIds = new List<string>();
        Dictionary<string, JSONNode> adData = new Dictionary<string, JSONNode>();

        public SAAdPopup adWindow;

        public bool useLocationServices = true;

        public bool FullScreenAdIsActive
        {
            get
            {
                bool anyVisible = false;
                foreach (DynamicPopupAd ad in fullscreenWindows)
                {
                    if (ad.IsVisible) anyVisible = true;
                }
                return anyVisible;
            }
        }

        bool findingAds;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(transform.gameObject);
            }

            //DontDestroyOnLoad(transform.gameObject);
            adData = new Dictionary<string, JSONNode>();
        }

        public void Start()
        {
            if (useLocationServices)
            {
                //SkillArcadeUIManager.instance.ShowLocationPermissionWindow();
               // Net.LocationService.instance.Initialize();
            }

            for (int i = 0; i < mainMenuAds.Count; i++)
            {
                ads.Add(mainMenuAds[i]);
                if (!adIds.Contains(ads[i].AdID))
                {
                    adIds.Add(ads[i].AdID);
                }
            }


            if (!Net.WebComms.loggingIn)
                UpdateAdData();
        }

        public void SearchForAds()
        {
            ads.Clear();
            for (int i = 0; i < mainMenuAds.Count; i++)
            {
                ads.Add(mainMenuAds[i]);
            }

            StartCoroutine(waitForSearch());
        }

        IEnumerator waitForSearch()
        {
            findingAds = true;
            yield return new WaitForSeconds(0.5f);
            DynamicAd[] foundAds = FindObjectsOfType<DynamicAd>();

            for (int i = 0; i < foundAds.Length; i++)
            {
                if (!ads.Contains(foundAds[i]))
                {
                    ads.Add(foundAds[i]);
                }
            }

            for (int i = 0; i < ads.Count; i++)
            {
                if (!adIds.Contains(ads[i].AdID))
                {
                    adIds.Add(ads[i].AdID);
                }
            }
            yield return null;
            findingAds = false;
            yield return null;
        }

        public void UpdateAdData(bool hardRefresh = false)
        {
            StartCoroutine(updateRoutine(hardRefresh));
        }

        IEnumerator updateRoutine(bool hardRefresh = false)
        {
            while (findingAds)
                yield return null;

            if (hardRefresh)
                adData.Clear();

            for (int i = 0; i < adIds.Count; i++)
            {
                if (!adData.ContainsKey(adIds[i]))
                {
                    Net.WebComms.SendGetAdRequest(adIds[i]);
                }
            }

            UpdateFromSavedData();
            yield return null;
        }

        public void LogIn()
        {
            Debug.LogWarning("User Logged In");
            UpdateAdData(true);
        }

        public void LogOut()
        {
            Debug.LogWarning("User Logged Out");
            adData.Clear();

            for (int i = 0; i < ads.Count; i++)
            {
                ads[i].SetToDefaults();
            }
        }

        public void ProcessReturnData(JSONNode data)
        {
            adData[data["adid"]] = data;

            string adID = data["adid"].ToString().Replace("\"", "");

            for (int i = 0; i < ads.Count; i++)
            {
                if (ads[i].AdID == adID)
                {
                    ads[i].ProcessReturnData(data);
                }
            }
        }

        /// <summary>
        /// Downloads the ad images by starting a coroutine and passing the texture to a callback function defined by DynamicAd implementations.
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="callback">Defined by the calling DynamicAd implementation to handle the downloaded texture</param>
        public void DownloadTexture(string URL, System.Action<Texture> callback)
        {
            StartCoroutine(downloadTexture(URL, callback));
        }

        IEnumerator downloadTexture(string URL, System.Action<Texture> callback)
        {
            Texture texture = null;

            if (URL.Contains("http"))
            {
                //try to load badge image URL as texture
                WWW www = new WWW(URL);
                while (!www.isDone)
                {
                    yield return null;
                }

                if (www.error != null)
                    Debug.Log("Problem downloading image: " + www.error);
                else
                    texture = www.texture;
            }
            else
            {
                yield break;
            }

            callback(texture);
        }

        public void UpdateFromSavedData()
        {
            for (int i = 0; i < ads.Count; i++)
            {
                if (adData.ContainsKey(ads[i].AdID))
                {
                    ads[i].ProcessReturnData(adData[ads[i].AdID]);
                }
            }
        }

        public void ShowAdWindow(DynamicAd ad)
        {
            adWindow.ShowAd(ad.loadedSecondaryImage, ad.loadedText, ad.loadedSponsorURL);
        }

        public void ShowFullScreenAd(string adId)
        {
            for (int i = 0; i < ads.Count; i++)
            {
                if (ads[i].AdID == adId)
                {
                    DynamicPopupAd popUpAd = (DynamicPopupAd)ads[i];
                    SkillArcadeUIManager.instance.overlayFade.Darken();
                    popUpAd.ShowAd();
                    return;
                }
            }
            Debug.LogWarning("No full-screen ad found for: " + adId);
        }

        public void CloseFullscreenAds()
        {
            for (int i = 0; i < fullscreenWindows.Count; i++)
            {
                fullscreenWindows[i].HideAd();
            }
        }

        public void HideMainMenuAds()
        {
            for (int i = 0; i < mainMenuAds.Count; i++)
            {
                mainMenuAds[i].HideAd();
            }
        }

        public void ShowMainMenuAds(float delay = 0)
        {
            StartCoroutine(delayAds(delay));
        }

        IEnumerator delayAds(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            for (int i = 0; i < mainMenuAds.Count; i++)
            {
                mainMenuAds[i].OpenAd();
            }
        }
    }
}
