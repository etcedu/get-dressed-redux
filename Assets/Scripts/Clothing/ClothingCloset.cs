using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/ClothingCloset", fileName = "New Clothing Closet")]
[System.Serializable]
public class ClothingCloset : ScriptableObject
{
    public TextAsset clothingDataFile;
    public List<ClothingPiece> clothingPieces;
    public List<ClothingModelConnection> connections;

    public ClothingPiece GetClothingPiece(string name)
    {
        return clothingPieces.Find(x => x.Tag == name);
    }

    #region Importing
#if UNITY_EDITOR
    [ExecuteInEditMode]
    [ContextMenu("Load Clothing")]
    public void LoadClothing()
    {
        LoadConnections();

        clothingPieces.Clear();
        clothingPieces = new List<ClothingPiece>();
        ParseClothes(clothingDataFile);
    }

    [ExecuteInEditMode]
    public void ParseClothes(TextAsset tsvData)
    {
        if (tsvData == null)
            return;

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

            c.DisplayName = fields[3];
            c.FeedbackName = fields[4];

            string[] tiers = fields[5].Split(new char[] { ';' }).Trim();
            c.Tiers = new();
            foreach (string tier in tiers)
            {
                if (Enum.TryParse(tier.ToUpper().Replace(' ', '_'), out Tier tierParsed))
                    c.Tiers.Add(tierParsed);
                else
                    Debug.LogError($"Error parsing category {tier} on line {i} ");
            }

            string goodFeedback = fields[6];
            c.GoodFeedback = goodFeedback;
            string okFeedback = fields[7];
            c.OKFeedback = okFeedback;
            string badFeedback = fields[8];
            c.BadFeedback = badFeedback;

            clothingPieces.Add(c);

            ClothingModelConnection connection = connections.Find(x => x.name == c.Tag);
            if (connection == null)
                Debug.LogError($"No ClothingModelConnection found for {c.DisplayName} at line {i}");
            else
                c.Connection = connection;

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    [ExecuteInEditMode]
    [ContextMenu("Load Connections")]
    public void LoadConnections()
    {
        connections.Clear();
        connections = new();
               
        string[] guids = AssetDatabase.FindAssets("t:ClothingModelConnection");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            connections.Add(AssetDatabase.LoadAssetAtPath<ClothingModelConnection>(path));
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

    }
#endif
    #endregion

}
