using UnityEngine;

namespace Simcoach.SkillArcade.UI
{
    public class LoadingPanel : MonoBehaviour
    {

        TweenAlpha_SA showTween;
        CanvasGroup canvasGroup;

        // Use this for initialization
        void Awake()
        {
            showTween = GetComponent<TweenAlpha_SA>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            canvasGroup.blocksRaycasts = true;
            showTween.PlayForward();
        }

        public void Hide()
        {
            canvasGroup.blocksRaycasts = false;
            showTween.PlayReverse();
        }
    }
}