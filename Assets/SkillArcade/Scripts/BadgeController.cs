using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Simcoach.SkillArcade
{

    public class Badge
    {
        public Badge(string _title, string _desc, string _title_sp, string _desc_sp, Texture2D _image)
        {
            title = _title;
            desc = _desc;
            title_sp = _title_sp;
            desc_sp = _desc_sp;
            image = _image;
        }

        public string title;
        public string title_sp;
        public string desc;
        public string desc_sp;
        public Texture2D image;
    }

    public class BadgeController : MonoBehaviour
    {

        public static BadgeController instance;
        public static bool loadingBadges;

        public List<Badge> currentBadgesQueue;

        /// <summary>
        /// Create current instance.
        /// Place all initialization code here.
        /// </summary>
        public static void CreateInstance()
        {
            // create instance
            if (instance == null)
            {
                GameObject obj = new GameObject();
                obj.name = "BadgeControllerGhost";
                instance = obj.AddComponent<BadgeController>();
                if (instance.currentBadgesQueue == null)
                {
                    instance.currentBadgesQueue = new List<Badge>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
        }

        public static void AddNewBadge(string _title, string _desc, string _title_sp, string _desc_sp, string imageURL)
        {
            CreateInstance();
            instance.StartCoroutine(instance.loadBadgeImage(imageURL, new Badge(_title, _desc, _title_sp, _desc_sp, null)));
        }
        
        public static Badge GetLatestBadge()
        {
            CreateInstance();
            if (instance.currentBadgesQueue.Count > 0)
            {
                Debug.Log("Found a badge: " + instance.currentBadgesQueue[0].title);
                return instance.currentBadgesQueue[0];
            }
            else
            {
                Debug.Log("Found no badges: ");
                return null;
            }
        }

        public static void ConfirmedViewedBadge(Badge viewedBadge)
        {
            if (instance.currentBadgesQueue.Count > 0 && instance.currentBadgesQueue.Contains(viewedBadge))
            {
                Debug.Log("Confirmed: " + viewedBadge);
                instance.currentBadgesQueue.Remove(viewedBadge);
            }
        }

        public static bool CheckForQueuedBadges()
        {
            CreateInstance();
            return (instance.currentBadgesQueue.Count > 0 && instance.currentBadgesQueue[0] != null);
        }

        IEnumerator loadBadgeImage(string URL, Badge newBadge)
        {
            loadingBadges = true;

            //Load from resources if not an url
            if (URL.Contains("http"))
            {
                //try to load badge image URL as texture
                WWW www = new WWW(URL);
                while (!www.isDone)
                {
                    yield return null;
                }

                if (www.error != null)
                    Debug.Log("Problem downloading badge image: " + www.error);
                else
                    newBadge.image = www.texture;

            }
            else
            {
                Texture2D badgeTex = Resources.Load<Texture2D>(URL);
                while (badgeTex == null)
                {
                    yield return null;
                }
                newBadge.image = badgeTex;
            }

            currentBadgesQueue.Add(newBadge);
            Debug.Log("Loaded new badge: " + newBadge.title);
            loadingBadges = false;
        }
    }
}