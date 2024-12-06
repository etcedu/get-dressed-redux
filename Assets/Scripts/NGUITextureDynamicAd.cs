using UnityEngine;
using Simcoach.SkillArcade;
using Simcoach.SimpleJSON;

public class NGUITextureDynamicAd : DynamicAd
{
    public UITexture nguiTexture;
    public OpenURL openURL;

    public override void Start()
    {
        nguiTexture = GetComponent<UITexture>();
        openURL = GetComponent<OpenURL>();
        base.Start();
    }

    public override void SetToDefaults()
    {
        base.SetToDefaults();
        nguiTexture.mainTexture = defaultMainImage;
    }

    public override void ProcessReturnData(JSONNode data)
    {
        base.ProcessReturnData(data);
    }

    public override void SetMainTexture(Texture texture)
    {
        base.SetMainTexture(texture);
        nguiTexture.mainTexture = loadedMainImage;
    }

    public override void SetSecondaryTexture(Texture texture)
    {
        base.SetSecondaryTexture(texture);
    }

    public void OnClick()
    {
        if (openURL != null && !string.IsNullOrEmpty(loadedSponsorURL))
        {
            openURL.URL = loadedSponsorURL;
            openURL.pageTitle = loadedText;
            openURL.Launch();
        }
    }
}
