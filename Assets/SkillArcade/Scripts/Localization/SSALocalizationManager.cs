using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

namespace Simcoach.SkillArcade
{

    public enum Language
    {
        ENGLISH, SPANISH
    }

    [System.Serializable]
    public class LocalizationData
    {
        public LocalizationItem[] items;
    }

    [System.Serializable]
    public class LocalizationItem
    {
        public string Key;
        public string English;
        public string Spanish;
    }

    public class SSALocalizationManager : MonoBehaviour
    {

        public static SSALocalizationManager instance;
        public GameObject canvas;

        public SSALocalizedText[] allTextObjects;
        private Dictionary<string, List<string>> localizedText;
        private bool isReady = false;
        private string missingTextString = "Localized text not found";

        private Language _shownLanguage;

        public Language GameLanguage
        {
            get { return _shownLanguage; }
            set { _shownLanguage = value; }
        }

        // Use this for initialization
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            GetAllTextObjects();

            SystemLanguage sysLanguage = Application.systemLanguage;
            if (sysLanguage == SystemLanguage.Spanish)
            {
                GameLanguage = Language.SPANISH;
                PlayerPrefs.SetInt("Language", (int)Language.SPANISH);
            }
            else
            {
                GameLanguage = Language.ENGLISH;
                PlayerPrefs.SetInt("Language", (int)Language.ENGLISH);
            }
            GameLanguage = Language.ENGLISH;

            //DontDestroyOnLoad(gameObject);
            LoadLocalizedText("SSALocalizationFile");
        }

        public void Start()
        {
            SetAllTextObjects();
        }

        public void SetGameLanguage(int currentLanguage)
        {
            SSALocalizationManager.instance.GameLanguage = (Language)currentLanguage;
            PlayerPrefs.SetInt("Language", currentLanguage);
            SetAllTextObjects();
        }

        public void GetAllTextObjects()
        {
            allTextObjects = canvas.GetComponentsInChildren<SSALocalizedText>();
        }

        public void SetAllTextObjects()
        {
            for (int i = 0; i < allTextObjects.Length; i++)
            {
                allTextObjects[i].SetText();
            }
        }

        public void LoadLocalizedText(string fileName)
        {
            localizedText = new Dictionary<string, List<string>>();
            string languageJson = Resources.Load<TextAsset>(fileName).text;

            if (languageJson != null)
            {
                LocalizationData data = JsonUtility.FromJson<LocalizationData>(languageJson);

                for (int i = 0; i < data.items.Length; i++)
                {
                    List<string> localizations = new List<string>();
                    localizations.Add(data.items[i].English);
                    localizations.Add(data.items[i].Spanish);
                    try
                    {
                        localizedText.Add(data.items[i].Key, localizations);
                    }
                    catch (System.ArgumentException e)
                    {
                        Debug.LogError(data.items[i].Key);
                        Debug.LogError(e.Data.ToString());
                    }
                }

                Debug.Log("Data loaded, dictionary contains: " + localizedText.Count + " entries");
            }
            else
            {
                Debug.LogError("Cannot find file!");
            }

            isReady = true;
        }

        public string GetLocalizedValue(string key)
        {
            string result = missingTextString;
            if (localizedText.ContainsKey(key))
            {
                List<string> translations = localizedText[key];
                result = translations[(int)_shownLanguage];
            }

            return result;

        }

        public bool GetIsReady()
        {
            return isReady;
        }

    }
}