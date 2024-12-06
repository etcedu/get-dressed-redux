using UnityEngine;

namespace Simcoach.SkillArcade.UI
{
    public class AlphaSetsInteractable : MonoBehaviour
    {

        public CanvasGroup canvasGroup;

        // Use this for initialization
        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        // Update is called once per frame
        void Update()
        {
            canvasGroup.blocksRaycasts = (canvasGroup.alpha > 0);
            canvasGroup.interactable = (canvasGroup.alpha > 0);
        }
    }
}