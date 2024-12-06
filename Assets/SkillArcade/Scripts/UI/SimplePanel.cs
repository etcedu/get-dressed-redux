using UnityEngine;
using System.Collections;
namespace Simcoach.SkillArcade.UI
{
    public class SimplePanel : MonoBehaviour
    {

        private UITweener_SA showTween;

        public bool showing;

        // Use this for initialization
        void Start()
        {
            SetTween();
        }

        void SetTween()
        {
            if (showTween == null)
                showTween = GetComponent<UITweener_SA>();
        }

        public void Toggle()
        {
            SetTween();
            showing = !showing;
            showTween.Toggle();
        }

        public void Show()
        {
            SetTween();
            showing = true;
            showTween.PlayForward();
        }

        public void Hide()
        {
            SetTween();
            showing = false;
            showTween.PlayReverse();
        }
    }
}