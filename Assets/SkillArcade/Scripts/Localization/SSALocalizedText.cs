using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Simcoach.SkillArcade;

namespace Simcoach.SkillArcade
{

    public class SSALocalizedText : MonoBehaviour
    {
        public string key;
        public int englishSize, spanishSize;
        public bool isSized;
        public bool isChangedPosition;
        public Vector2 englishPosition, spanishPosition;
        public bool suppress;

        private void Start()
        {

        }

        public void SetKey(string newKey)
        {
            bool reload = (key != newKey);

            key = newKey;

            SetText();
        }

        public void SetText()
        {
            if (suppress)
                return;

            string localizedText = SSALocalizationManager.instance.GetLocalizedValue(key);

            if (!string.IsNullOrEmpty(localizedText))
                GetComponent<Text>().text = localizedText;

            if (key == "Email1" || key == "Email2")
            {
                GetComponent<Text>().text = GetComponent<Text>().text.Replace("{address}", CurrentSkiller.emailAddr);
            }

            if (isSized)
            {
                int size = 0;
                if (SSALocalizationManager.instance.GameLanguage == Language.ENGLISH)
                {
                    size = englishSize;
                }
                else
                {
                    size = spanishSize;
                }

                GetComponent<Text>().fontSize = size;
            }

            if (isChangedPosition)
            {
                Vector2 pos;
                if (SSALocalizationManager.instance.GameLanguage == Language.ENGLISH)
                {
                    pos = englishPosition;
                }
                else
                {
                    pos = spanishPosition;
                }

                transform.localPosition = pos;
            }
        }
    }
}