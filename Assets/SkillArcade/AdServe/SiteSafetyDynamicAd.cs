using Simcoach.SkillArcade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simcoach.SimpleJSON;

namespace Simcoach.SkillArcade
{
    public class SiteSafetyDynamicAd : DynamicAd
    {
        //public UITexture environmentObject;
        
        public override void SetToDefaults()
        {
            base.SetToDefaults();
           // environmentObject.mainTexture = loadedMainImage;
        }

        public override void SetMainTexture(Texture texture)
        {
            base.SetMainTexture(texture);
            //environmentObject.mainTexture = loadedMainImage;
        }
    }
}