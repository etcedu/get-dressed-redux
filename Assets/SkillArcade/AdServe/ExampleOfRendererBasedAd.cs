using System.Collections;
using System.Collections.Generic;
using Simcoach.SimpleJSON;
using UnityEngine;

namespace Simcoach.SkillArcade
{
    public class ExampleOfRendererBasedAd : DynamicAd
    {
        new MeshRenderer renderer;
        Texture[] textures;

        // Use this for initialization
        override public void Start()
        {
            renderer = GetComponent<MeshRenderer>();
            base.Start();
        }

        public override void SetToDefaults()
        {
            base.SetToDefaults();
            renderer.material.mainTexture = (Texture)defaultMainImage;
        }

        public override void ProcessReturnData(JSONNode data)
        {
            base.ProcessReturnData(data);
        }

        public override void SetMainTexture(Texture texture)
        {
            base.SetMainTexture(texture);
            renderer.material.mainTexture = texture;
        }

        public override void SetSecondaryTexture(Texture texture)
        {
            base.SetSecondaryTexture(texture);
        }

        public void OnTap()
        {
            AdClicked();
        }

        public void AdClicked()
        {
            Debug.Log("CLick");
            DynamicAdManager.instance.ShowAdWindow(this);
        }
    }
}