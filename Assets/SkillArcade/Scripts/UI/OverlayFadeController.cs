using UnityEngine;
using UnityEngine.UI;

namespace Simcoach.SkillArcade.UI
{
    public class OverlayFadeController : MonoBehaviour
    {

        public TweenAlpha_SA fadeTween;
        public CanvasGroup canvasGroup;
        public Image darkImage;


        //For odler games that use NGUI, you need a fade panel in the ngui UI Root with a collider that will block clicks, and
        //a TweenAlpha assigned here.

        //public TweenAlpha fadeTween_NGUI;


        public void Darken()
        {
            //if (fadeTween_NGUI == null)
            //    fadeTween_NGUI = GameObject.Find("SkillArcadeFade").GetComponent<TweenAlpha>();

            //fadeTween_NGUI.PlayForward();

            fadeTween.PlayForward();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            darkImage.raycastTarget = true;
        }

        public void Undarken()
        {
            //if (fadeTween_NGUI == null)
            //    fadeTween_NGUI = GameObject.Find("SkillArcadeFade").GetComponent<TweenAlpha>();

            //fadeTween_NGUI.PlayReverse();

            fadeTween.PlayReverse();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            darkImage.raycastTarget = false;
        }
    }
}