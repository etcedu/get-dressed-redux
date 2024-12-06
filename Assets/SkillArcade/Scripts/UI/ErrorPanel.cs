using UnityEngine;
using System.Collections;

namespace Simcoach.SkillArcade.UI
{
    public class ErrorPanel : MonoBehaviour
    {

        TweenAlpha_SA showTween;
        CanvasGroup canvasGroup;
        public UnityEngine.UI.Text errorLabel;

        // Use this for initialization
        void Start()
        {
            showTween = GetComponent<TweenAlpha_SA>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(string errorMessage)
        {
            errorLabel.text = errorMessage;
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