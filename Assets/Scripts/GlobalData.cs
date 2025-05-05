using System.Collections.Generic;
using UnityEngine;


public enum Gender
{
    EITHER, MALE, FEMALE
}
public enum Category
{
    HEAD, TOP, BOTTOM, SHOES
}
public enum TierEnum
{
    INFORMAL, CASUAL, BUSINESS_CASUAL, BUSINESS_PROFESSIONAL
}


public class GlobalData : MonoBehaviour
{
    public static GlobalData Instance { get; private set; }

    public static CharacterData currentCharacterSelection;

    [SerializeField] List<CharacterData> characters = new List<CharacterData>();

    public void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        currentCharacterSelection = null;
    }

    public static void SetCharacter(CharacterData charData)
    {
        currentCharacterSelection = charData;
    }

    public static IList<CharacterData> GetCharacters()
    {
        return Instance.characters;
    }
}