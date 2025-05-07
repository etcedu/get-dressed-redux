using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class DressingManager : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] private float clothingAnimationProbability = .2f;
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private RuntimeAnimatorController maleController;
    [SerializeField] private RuntimeAnimatorController femaleController;

    [SerializeField] List<Material> headMaterials = new();
    [SerializeField] List<Material> topMaterials = new();
    [SerializeField] List<Material> bottomMaterials = new();
    [SerializeField] List<Material> feetMaterials = new();

    /// Functions ///

    #region Setup
    // Populate the grids and set up interactions
    void Awake()
    {

    }

    // Establish starting category
    void Start()
    {
        ClearClothingFromCategory(Category.HEAD);
        ClearClothingFromCategory(Category.TOP);
        ClearClothingFromCategory(Category.BOTTOM);
        ClearClothingFromCategory(Category.FEET);
    }
    #endregion

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
    }
   

    void PlayAnimation(string trigger)
    {
        if (UnityEngine.Random.Range(0f, 1f) > clothingAnimationProbability || characterAnimator.GetBool("Playing Animation"))
            return;

        characterAnimator.SetBool("Playing Animation", true);
        characterAnimator.SetTrigger(trigger);
    }

    // Evaluate the clothing selection
    public void Evaluate()
    {
        Debug.Log($"Selected Head: {GlobalData.selectedHeadPiece.DisplayName}");
        Debug.Log($"Selected Top: {GlobalData.selectedTopPiece.DisplayName}");
        Debug.Log($"Selected Bottom: {GlobalData.selectedBottomPiece.DisplayName}");
        Debug.Log($"Selected Feet: {GlobalData.selectedFeetPiece.DisplayName}");

        TimeSpan timeElapsed = (CrossSceneInfo.LevelStartedTimeStamp - DateTime.Now).Duration();
        float secondsElapsed = (float)timeElapsed.TotalSeconds;
        //EventRecorder.RecordLevelCompleted(CrossSceneInfo.LevelAttemptId, CrossSceneInfo.LastCompanyName, CrossSceneInfo.LastPositionName, passed, _faceScore, _topScore, _bottomScore, _shoesScore, _itemScore, _score, CrossSceneInfo.PassingCuttoff, secondsElapsed);
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

