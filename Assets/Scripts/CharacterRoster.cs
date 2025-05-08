using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CharacterRoster", fileName = "New Character Roster")]
[System.Serializable]
public class CharacterRoster : ScriptableObject
{
    public TextAsset charactersDataFile;
    public List<CharacterData> characters;

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
        ParseClothes(charactersDataFile);
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



            characters.Add(c);

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
