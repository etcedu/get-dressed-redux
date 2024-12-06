using UnityEngine;
using System.Collections;

namespace Simcoach.SkillArcade.UI
{
    public class SA_Darkener_NGUI : MonoBehaviour
    {

        public Simcoach.SkillArcade.TweenAlpha_SA tween;

        public void Darken()
        {
            tween.PlayForward();
        }

        public void UnDarken()
        {
            tween.PlayReverse();
        }
    }
}