using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEditor;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Custom/CharacterRoster", fileName = "New Character Roster")]
[System.Serializable]
public class CharacterRoster : ScriptableObject
{
    public TextAsset charactersDataFile;
    public List<CharacterData> characters;
    public ClothingCloset clothingCloset;

    public CharacterData GetCharacter(string characterTag)
    {
        return characters.Find(x => x.characterTag == characterTag);
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    [ContextMenu("Load Characters")]
    public void LoadCharacters()
    {
        characters.Clear();
        characters = new();
        ParseCharacters(charactersDataFile);
    }

    [ExecuteInEditMode]
    [ContextMenu("Load Combined Data")]
    public void LoadCombinedData()
    {
        characters.Clear();
        characters = new();
        ParseCombined(charactersDataFile);
    }

    [ExecuteInEditMode]
    public void ParseCharacters(TextAsset tsvData)
    {
        if (tsvData == null)
            return;

        string[] items = tsvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < items.Length; i++)
        {
            string[] fields = items[i].Split(new char[] { '\t' });

            CharacterData c = new CharacterData();

            c.characterTag = fields[0];
            c.characterName = fields[1];

            if (Enum.TryParse(fields[2].ToUpper(), out Gender genderParsed))
                c.gender = genderParsed;
            else
                Debug.LogError($"Error parsing gender {fields[2]} on line {i} ");

            c.jobTitle = fields[3];
            c.description = fields[4];
            c.jobAttireDescription = fields[5];
            c.imageAssetPath = fields[6];

            c.headOptions = fields[7].Split(new char[] { ';' }).Trim();
            c.topOptions = fields[8].Split(new char[] { ';' }).Trim();
            c.bottomOptions = fields[9].Split(new char[] { ';' }).Trim();
            c.feetOptions = fields[10].Split(new char[] { ';' }).Trim();
            c.otherClothes = fields[11].Split(new char[] { ';' }).Trim();
            
            if (ColorExtensions.TryParseHexStringRGBA(fields[12], out Color32 skinColor))
                c.skinColor = skinColor;
            else
                c.skinColor = Color.white;

            c.winFeedback = fields[13];
            c.loseFeedback = fields[14];

            characters.Add(c);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
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

                currentCharacterLoading.otherClothes = fields[8].Split(new char[] { ';' }).Trim();

                if (ColorExtensions.TryParseHexStringRGBA(fields[9], out Color32 skinColor))
                    currentCharacterLoading.skinColor = skinColor;
                else
                    currentCharacterLoading.skinColor = Color.white;

                currentCharacterLoading.winFeedback = fields[10];
                currentCharacterLoading.loseFeedback = fields[11];
                              

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
                        Debug.LogError($"Error parsing category {tier} on line {i} ");
                }

                c.GoodFeedback = fields[7];
                c.OKFeedback = fields[8];
                c.BadFeedback = fields[9];

                ClothingModelConnection connection = clothingCloset.connections.Find(x => x.name == c.Tag);
                if (connection == null)
                    Debug.LogError($"No ClothingModelConnection found for {c.DisplayName} at line {i}");
                else
                    c.Connection = connection;

                if (c.Category == Category.HEAD)
                    characters[characters.Count - 1].headPieces.Add(c);
                else if (c.Category == Category.TOP)
                    characters[characters.Count - 1].topPieces.Add(c);
                else if (c.Category == Category.BOTTOM)
                    characters[characters.Count - 1].bottomPieces.Add(c);
                else if (c.Category == Category.FEET)
                    characters[characters.Count - 1].feetPieces.Add(c);
            }            

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
