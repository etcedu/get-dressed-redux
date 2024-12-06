using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Simcoach.SkillArcade.UI
{

    public class SABadgePopup : MonoBehaviour
    {

        public SkillArcadeUIManager SAManager;
        public SimplePanel popupPanel;
        public Image badgeImage;
        public Text badgeTitleLabel;
        public Text badgeDescLabel;

        Badge currentBadge;
        bool lastBadgeWasOfflineMode;

        public void ShowBadge(Badge newBadge, bool offline = false)
        {
            currentBadge = newBadge;
            var imageRect = new Rect(0, 0, newBadge.image.width, newBadge.image.height);
            badgeImage.sprite = Sprite.Create(newBadge.image, imageRect, new Vector2(0.5f, 0.5f));


            //Localization
            if (SSALocalizationManager.instance.GameLanguage == Language.SPANISH)
            {
                badgeTitleLabel.text = newBadge.title_sp;
                badgeDescLabel.text = newBadge.desc_sp;
            }
            else //English
            {
                badgeTitleLabel.text = newBadge.title;
                badgeDescLabel.text = newBadge.desc;
            }


            lastBadgeWasOfflineMode = offline;
            popupPanel.Show();
        }

        public void ContinueButton_OnTap()
        {
            if (!lastBadgeWasOfflineMode)
            {
                BadgeController.ConfirmedViewedBadge(currentBadge);
                currentBadge = null;
            }
            SAManager.BadgePopupContinueButton_OnTap();
        }

        public void Hide()
        {
            popupPanel.Hide();
        }

    }
}
