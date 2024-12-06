using UnityEngine;
using UnityEngine.UI;

namespace Simcoach.SkillArcade.UI
{
    public class SkillArcadeButton : MonoBehaviour
    {

        public Button _button;
        public Image _buttonImage;
        public Text _initialsText;
        public TweenAlpha_SA _initialsTween;

        bool visible;

        public void SetTouchable(bool touchable)
        {
            _button.interactable = touchable;
            _buttonImage.raycastTarget = touchable;
        }

        public void SetInitialsText(string initials)
        {
            _initialsText.text = initials;
        }

        public void ShowInitials()
        {
            _initialsTween.PlayForward();
        }

        public void HideInitials()
        {
            _initialsTween.PlayReverse();
        }

        public void PlayAppearAnimation()
        {
            if (!visible)
                GetComponent<Animator>().Play("LoginButtonAppear");
            visible = true;

            SetTouchable(true);
        }

        public void PlayDisappearAnimation()
        {
            if (visible)
            {
                GetComponent<Animator>().Play("LoginButtonDisappear");
                HideInitials();
            }
            visible = false;

            SetTouchable(false);
        }
    }
}