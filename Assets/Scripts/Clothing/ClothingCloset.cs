using Clothing;
using Simcoach.SkillArcade;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingCloset : MonoBehaviour
{
    public List<ClothingPiece> head = new List<ClothingPiece>();
    public List<ClothingPiece> top = new List<ClothingPiece>();
    public List<ClothingPiece> bottom = new List<ClothingPiece>();
    public List<ClothingPiece> feet = new List<ClothingPiece>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Importing
    #if UNITY_EDITOR
    public void ParseClothes(TextAsset tsvData, bool clearItemsOnImport)
    {
        if (tsvData == null)
            return;

        if (clearItemsOnImport)
        {
            head = new List<ClothingPiece>();
            top = new List<ClothingPiece>();
            bottom = new List<ClothingPiece>();
            feet = new List<ClothingPiece>();
        }

        string[] items = tsvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < items.Length; i++)
        {
            string[] fields = items[i].Split(new char[] { '\t' });
            
            ClothingPiece c = new ClothingPiece();

            string id = fields[0];
            c.Tag = id;

            string category = fields[1];
            if (Enum.TryParse(category.ToUpper(), out Category categoryParsed))
                c.Category = categoryParsed;
            else
                Debug.LogError($"Error parsing category {category} on line {i} ");

            string gender = fields[2];
            if (Enum.TryParse(gender.ToUpper(), out Gender genderParsed))
                c.GenderRole = genderParsed;
            else
                Debug.LogError($"Error parsing gender {gender} on line {i} ");

            string humanName = fields[3];
            c.DisplayName = humanName;


            string[] tiers = fields[4].Split(new char[] { ';' }).Trim();
            foreach (string tier in tiers)
            {
                if (Enum.TryParse(tier.ToUpper().Replace(' ', '_'), out Tier tierParsed))
                    c.Tiers.Add(tierParsed);
                else
                    Debug.LogError($"Error parsing category {tier} on line {i} ");
            }           
        }
    }
    #endif
    #endregion
}   