using System;
using UnityEngine;
using System.Collections;
using System.Diagnostics;


#if UNITY_EDITOR
#endif
using System.Collections.Generic;

[System.Serializable]
public class DressingManager : MonoBehaviour
{
    [SerializeField] DressingUI dressingUI;
    [SerializeField] SimpleFeedback feedbackUI;

    [SerializeField] [Range(0f, 1f)] private float clothingAnimationProbability = .2f;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private RuntimeAnimatorController maleController;
    [SerializeField] private RuntimeAnimatorController femaleController;

    [SerializeField] List<Material> headMaterials = new();
    [SerializeField] List<Material> topMaterials = new();
    [SerializeField] List<Material> bottomMaterials = new();
    [SerializeField] List<Material> feetMaterials = new();
    [SerializeField] List<Material> otherMaterials = new();


    public BodyPartRender[] maleSetup = new BodyPartRender[0];
    public BodyPartRender[] femaleSetup = new BodyPartRender[0];

    [SerializeField] SoundVolumePair[] clothSounds;
    [SerializeField] SoundVolumePair[] hairSounds;
    SFXManager sfxManager;

    Stopwatch stopwatch;

    IEnumerator Start()
    {
        GlobalData.inFeedback = false;

        sfxManager = FindObjectOfType<SFXManager>();

        ClearClothingFromCategory(Category.HEAD);
        ClearClothingFromCategory(Category.TOP);
        ClearClothingFromCategory(Category.BOTTOM);
        ClearClothingFromCategory(Category.FEET);
        ClearClothingFromCategory(Category.OTHER);

        SetupCharacter();
        dressingUI.Init();

        yield return new WaitForSeconds(1.45f);

        if (GlobalData.isTutorial)
            FindObjectOfType<TutorialManager>()?.StartTutorial();

        stopwatch = new Stopwatch();
        stopwatch.Start();

        EventRecorder.RecordLevelStartedEvent(GlobalData.currentCharacterSelection.characterName);
    }

    void SetupCharacter()
    {
        if (GlobalData.currentCharacterSelection == null)
            GlobalData.SetCharacter(GlobalData.GetCharacters()[0].characterTag);

        GlobalData.InitNewLevel();

        if (GlobalData.currentCharacterSelection.gender == Gender.MALE)
        {
            foreach (BodyPartRender bpr in maleSetup)
            {
                bpr.material.color = GlobalData.currentCharacterSelection.skinColor;
                bpr.renderer.sharedMaterial = bpr.material;
            }
            characterAnimator.runtimeAnimatorController = maleController;
        }
        else
        {
            foreach (BodyPartRender bpr in femaleSetup)
            {
                bpr.material.color = GlobalData.currentCharacterSelection.skinColor;
                bpr.renderer.sharedMaterial = bpr.material;
            }
            characterAnimator.runtimeAnimatorController = femaleController;
        }

        for (int i = 0; i < GlobalData.currentCharacterSelection.otherPieces.Count; i++)
        {
            SetClothing(GlobalData.currentCharacterSelection.otherPieces[i]);
        }

        GlobalData.completedLastCharacter = false;
    }

    public void ClearClothingFromCategory(Category clothingCategory)
    {
        switch (clothingCategory)
        {
            case Category.HEAD:
                for (int i = 0; i < headMaterials.Count; i++)
                    headMaterials[i].mainTexture = null;
                break;
            case Category.TOP:
                for (int i = 0; i < topMaterials.Count; i++)
                    topMaterials[i].mainTexture = null;
                break;
            case Category.BOTTOM:
                for (int i = 0; i < bottomMaterials.Count; i++)
                    bottomMaterials[i].mainTexture = null;
                break;
            case Category.FEET:
                for (int i = 0; i < feetMaterials.Count; i++)
                    feetMaterials[i].mainTexture = null;
                break;
            case Category.OTHER:
                for (int i = 0; i < otherMaterials.Count; i++)
                    otherMaterials[i].mainTexture = null;
                break;
            case Category.DRESS:
                for (int i = 0; i < topMaterials.Count; i++)
                    topMaterials[i].mainTexture = null;
                for (int i = 0; i < bottomMaterials.Count; i++)
                    bottomMaterials[i].mainTexture = null;
                break;
        }
    }

    public void ClearClothingFromConnection(ClothingModelConnection connection)
    {
        for (int i = 0; i < connection.maleMaterials.Count; i++)
        {
            if (i < connection.maleMaterials.Count)
                connection.maleMaterials[i].mainTexture = null;
        }
        for (int i = 0; i < connection.femaleMaterials.Count; i++)
        {
            if (i < connection.femaleMaterials.Count)
                connection.femaleMaterials[i].mainTexture = null;
        }
    }

    public void SetClothing(ClothingPiece clothingPiece)
    {
        GlobalData.SetClothingSelection(clothingPiece);
        ClothingModelConnection connection = clothingPiece.Connection;

        for (int i = 0; i < connection.maleMaterials.Count; i++)
        {
            if (i < connection.textures.Count)
                connection.maleMaterials[i].mainTexture = connection.textures[i];
        }
        for (int i = 0; i < connection.femaleMaterials.Count; i++)
        {
            if (i < connection.textures.Count)
                connection.femaleMaterials[i].mainTexture = connection.textures[i];
        }

        switch (clothingPiece.Category)
        {
            case Category.HEAD:
                PlayHeadAnimation();
                break;
            case Category.TOP:
                PlayTopAnimation();
                break;
            case Category.DRESS:
            case Category.BOTTOM:
                PlayBottomAnimation();
                break;
            case Category.FEET:
                PlayFeetAnimation();
                break;
            default:
                break;
        }
    }

    public void PlayHeadAnimation()
    {
        if (GlobalData.currentCharacterSelection.gender == Gender.FEMALE)
            PlayAnimation("Hair Changed");
    }

    public void PlayTopAnimation()
    {
        if (GlobalData.currentCharacterSelection.gender == Gender.MALE)
            characterAnimator.SetBool("Shirt Wipe", UnityEngine.Random.Range(0f, 1f) > .5f);
        PlayAnimation("Shirt Changed");
    }

    public void PlayBottomAnimation()
    {
        if (GlobalData.currentCharacterSelection.gender == Gender.FEMALE)
            characterAnimator.SetBool("Skirt", GlobalData.selectedTopPiece != null && (GlobalData.selectedTopPiece.Tag.Contains("Skirt")
                                                                                    || GlobalData.selectedTopPiece.Tag.Contains("Dress")));
        PlayAnimation("Pants Changed");
    }

    public void PlayFeetAnimation()
    {
        PlayAnimation("Shoes Changed");
    }

    public void PlayAnimation(string trigger)
    {
        if (UnityEngine.Random.Range(0f, 1f) > clothingAnimationProbability || characterAnimator.GetBool("Playing Animation"))
            return;

        characterAnimator.SetBool("Playing Animation", true);
        characterAnimator.SetTrigger(trigger);
    }

    public void PlayClothSound()
    {
        sfxManager?.PlayOneShot(clothSounds[UnityEngine.Random.Range(0, clothSounds.Length)]);
    }

    public void PlayHairSounds()
    {
        sfxManager?.PlayOneShot(hairSounds[UnityEngine.Random.Range(0, hairSounds.Length)]);
    }

    // Evaluate the clothing selection
    public void Evaluate()
    {
        CameraTrack2D.ResetTarget();
        stopwatch.Stop();
        if (GlobalData.isTutorial)
            EventRecorder.RecordCompletedTutorialEvent((float)stopwatch.Elapsed.TotalSeconds);

        dressingUI.Hide();
        feedbackUI.StartFeedback(stopwatch.Elapsed.TotalSeconds);
        GlobalData.inFeedback = true;
    }

    public void Restart()
    {
        stopwatch.Stop();
        EventRecorder.RecordLevelRestartedEvent((float)stopwatch.Elapsed.TotalSeconds, GlobalData.currentCharacterSelection.characterName);
        SceneLoader.LoadScene("Dressing");
    }

    public void ReturnToMenu()
    {
        EventRecorder.RecordLevelQuitEvent((float)stopwatch.Elapsed.TotalSeconds, GlobalData.currentCharacterSelection.characterName);
        SceneLoader.LoadScene("LevelSelection");
    }
     

    /// Extra ///

    #region SubClasses
    [System.Serializable]
    public class ClothingSetup
    {
        public List<Clothing.Info> clothing = new List<Clothing.Info>();
        public Renderer[] renderers = new Renderer[0];
    }


    [System.Serializable]
    public class OtherSetup
    {
        public string category;
        public List<Clothing.Info> clothing = new List<Clothing.Info>();
        public Renderer renderer;
        public Renderer otherRenderer;
    }

    [System.Serializable]
    public class TierCategoryFeedback
    {
        public CrossSceneInfo.GenderEnum gender;
        public List<Clothing.Tier.TierEnum> tiers = new List<Clothing.Tier.TierEnum>();
        public Clothing.Category.CategoryEnum category;
        public string feedback;
    }

    [System.Serializable]
    public class BodyPartRender
    {
        public Renderer renderer;
        public Material material;
    }
    #endregion
}
#pragma warning restore 0649

