using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CharacterRoster", fileName = "New Character Roster")]
[System.Serializable]
public class CharacterRoster : ScriptableObject
{
    public TextAsset charactersDataFile;
    public List<CharacterData> characters;
    public List<ClothingModelConnection> connections;

    public CharacterData GetCharacter(string characterTag)
    {
        return characters.Find(x => x.characterTag == characterTag);
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    [ContextMenu("Load Combined Data")]
    public void LoadCombinedData()
    {
        characters.Clear();
        characters = new();
        LoadConnections();
        ParseCombined(charactersDataFile);
    }
      
    [ExecuteInEditMode]
    public void ParseCombined(TextAsset tsvData)
    {
        if (tsvData == null)
            return;

        string[] items = tsvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < items.Length; i++)
        {
            string[] fields = items[i].Split(new char[] { '\t' });

            if (fields[0].ToLower() == "character")
            {
                CharacterData currentCharacterLoading = new CharacterData();
                currentCharacterLoading.characterTag = fields[1];
                currentCharacterLoading.characterName = fields[2];

                if (Enum.TryParse(fields[3].ToUpper(), out Gender genderParsed))
                    currentCharacterLoading.gender = genderParsed;
                else
                    Debug.LogError($"Error parsing gender {fields[2]} on line {i} ");

                currentCharacterLoading.jobTitle = fields[4];
                currentCharacterLoading.description = fields[5];
                currentCharacterLoading.jobAttireDescription = fields[6];
                currentCharacterLoading.imageAssetPath = fields[7];
                
                if (ColorExtensions.TryParseHexStringRGBA(fields[8], out Color32 skinColor))
                    currentCharacterLoading.skinColor = skinColor;
                else
                    currentCharacterLoading.skinColor = Color.white;

                currentCharacterLoading.winFeedback = fields[9];
                currentCharacterLoading.loseFeedback = fields[10];
                              

                characters.Add(currentCharacterLoading);
            }
            else
            {
                ClothingPiece c = new ClothingPiece();

                string id = fields[1];
                c.Tag = id;

                string category = fields[2];
                if (Enum.TryParse(category.ToUpper(), out Category categoryParsed))
                    c.Category = categoryParsed;
                else
                    Debug.LogError($"Error parsing category {category} on line {i} ");

                string gender = fields[3];
                if (Enum.TryParse(gender.ToUpper(), out Gender genderParsed))
                    c.GenderRole = genderParsed;
                else
                    Debug.LogError($"Error parsing gender {gender} on line {i} ");

                c.DisplayName = fields[4];
                c.FeedbackName = fields[5];

                string[] tiers = fields[6].Split(new char[] { ';' }).Trim();
                c.Tiers = new();
                foreach (string tier in tiers)
                {
                    if (Enum.TryParse(tier.ToUpper().Replace(' ', '_'), out Tier tierParsed))
                        c.Tiers.Add(tierParsed);
                    else
                        Debug.LogError($"No tier found for \"{tier}\" on line {i} ");
                }

                c.GoodFeedback = fields[7];
                c.OKFeedback = fields[8];
                c.BadFeedback = fields[9];

                ClothingModelConnection connection = connections.Find(x => x.name == c.Tag);
                if (connection == null)
                    Debug.LogError($"No ClothingModelConnection found for {c.DisplayName} at line {i}");
                else
                    c.Connection = connection;

                if (c.Category == Category.HEAD)
                    characters[characters.Count - 1].headPieces.Add(c);
                else if (c.Category == Category.TOP || c.Category == Category.DRESS)
                    characters[characters.Count - 1].topPieces.Add(c);
                else if (c.Category == Category.BOTTOM)
                    characters[characters.Count - 1].bottomPieces.Add(c);
                else if (c.Category == Category.FEET)
                    characters[characters.Count - 1].feetPieces.Add(c);
                else if (c.Category == Category.OTHER)
                    characters[characters.Count - 1].otherPieces.Add(c);
            }            

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
}
