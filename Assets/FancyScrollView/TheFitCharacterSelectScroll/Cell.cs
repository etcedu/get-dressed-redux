/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using EasingCore;
using FancyScrollView.Example09;
using TMPro;

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

        CharacterData data;

        public override void UpdateContent(CharacterData charData)
        {
            data = charData;
            image.texture = null;

            image.texture = Resources.Load<Texture>($"CharacterPortraits/{charData.imageAssetPath}");
            title.text = charData.characterName;
            description.text = charData.description;
            job.text = charData.jobTitle;

            SetLockState(!GlobalData.GetTutorialFinished() && !data.characterTag.ToLower().Contains("tutorial"));
            completedObject.SetActive(GlobalData.GetCharacterCompleted(data.characterTag));

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
        }
    }
}
