/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using TMPro;
using System.Collections;

namespace FancyScrollView.TheFitCharacterSelect
{
    class Cell : FancyCell<CharacterData>
    {
        readonly EasingFunction alphaEasing = Easing.Get(Ease.OutQuint);

        [SerializeField] TMP_Text title = default;
        [SerializeField] TMP_Text description = default;
        [SerializeField] TMP_Text job = default;
        [SerializeField] RawImage image = default;
        [SerializeField] Image background = default;
        [SerializeField] CanvasGroup canvasGroup = default;

        [SerializeField] Color normalColor;
        [SerializeField] Color fadedColor;

        [SerializeField] Image fader;
        [SerializeField] GameObject lockedObject;
        [SerializeField] GameObject completedObject;
        [SerializeField] UITweener starsTween;
        [SerializeField] CanvasGroup starsCG;
        [SerializeField] GameObject[] stars;
        [SerializeField] SoundVolumePair[] starSounds;


        CharacterData data;

        public string Test()
        {
            return data.characterName;
        }

        public override void UpdateContent(CharacterData charData)
        {
            data = charData;
            image.texture = null;

            image.texture = Resources.Load<Texture>($"CharacterPortraits/{charData.imageAssetPath}");
            title.text = charData.characterName;
            description.text = charData.description;
            job.text = charData.jobTitle;

            SetLockState(!GlobalData.GetTutorialFinished() && !data.characterTag.ToLower().Contains("tutorial"));
            //completedObject.SetActive(GlobalData.GetCharacterCompleted(data.characterTag));

            if (GlobalData.currentCharacterSelection?.characterTag == data.characterTag && GlobalData.setNewHighScore)
            {
                ShowStarsFirstTime();
                GlobalData.setNewHighScore = false;
            }
            else
                ShowStarsNoAnim();

            UpdateSibling();
        }

        void UpdateSibling()
        {
            var cells = transform.parent.Cast<Transform>()
                .Select(t => t.GetComponent<Cell>())
                .Where(cell => cell.IsVisible);

            if (Index == cells.Min(x => x.Index))
            {
                transform.SetAsLastSibling();
            }

            if (Index == cells.Max(x => x.Index))
            {
                transform.SetAsFirstSibling();
            }
        }

        public void SetLockState(bool locked)
        {
            lockedObject.SetActive(locked);
        }

        public void ShowStarsFirstTime()
        {
            StartCoroutine(starsFillRoutine());
        }

        bool inStarFillRoutine = false;
        IEnumerator starsFillRoutine()
        {
            inStarFillRoutine = true;
            FindObjectOfType<TheFitScrollView>().HideUI();
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].transform.localScale = Vector3.zero;
                stars[i].SetActive(false);
            }

            TheFitScrollView scrollView = FindObjectOfType<TheFitScrollView>();
            while (!scrollView.init)
                yield return null;

            yield return new WaitForSeconds(1.0f);

            int numStars = GlobalData.GetCharacterStars(data.characterTag);
            for (int i = 0; i < numStars; i++)
            {
                stars[i].SetActive(true);
                stars[i].GetComponent<UITweener>().PlayForward_FromBeginning();
                SFXManager.instance.PlayOneShot(starSounds[i]);
                yield return new WaitForSeconds(0.5f);
            }
            FindObjectOfType<TheFitScrollView>().ShowUI();
            inStarFillRoutine = false;
        }

        public void ShowStarsNoAnim()
        {
            if (inStarFillRoutine)
                return;

            for (int i = 0; i < stars.Length; i++)
                stars[i].SetActive(false);

            int numStars = GlobalData.GetCharacterStars(data.characterTag);

            for (int i = 0; i < numStars; i++)
            {
                stars[i].SetActive(true);
                stars[i].GetComponent<UITweener>().PlayForward();
            }
        }

        public override void UpdatePosition(float t)
        {
            const float popAngle = -15;
            const float slideAngle = 25;

            const float popSpan = 0.75f;
            const float slideSpan = 0.25f;

            t = 1f - t;

            var pop = Mathf.Min(popSpan, t) / popSpan;
            var slide = Mathf.Max(0, t - popSpan) / slideSpan;

            transform.localRotation = t < popSpan
                ? Quaternion.Euler(0, 0, popAngle * (1f - pop))
                : Quaternion.Euler(0, 0, slideAngle * slide);

            transform.localPosition = Vector3.left * 500f * slide;

            canvasGroup.alpha = alphaEasing(1f - slide);

            fader.color = Color.Lerp(fadedColor, normalColor, pop);
            if (pop > 0.9f)
            {
                starsCG.alpha = Mathf.Lerp(0, 1, (pop-0.9f) * 10f);
            }
            else
                starsCG.alpha = 0;
        }

        public void OnTap()
        {
            FindObjectOfType<TheFitScrollView>().TapOnCharacter();
        }

        public void ShowStars()
        {
            starsTween.PlayForward();
        }

        public void HideStars()
        {
            starsTween.PlayReverse();
        }
    }
}
