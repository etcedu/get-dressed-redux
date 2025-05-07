using System.Collections.Generic;
using UnityEngine;
using static CrossSceneInfo;


public enum Gender
{
    EITHER, MALE, FEMALE
}
public enum Category
{
    HEAD, TOP, BOTTOM, SHOES
}
public enum Tier
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

    #region Gender
    /// <summary>
    /// Gets the current gender, loading it from save if necessary
    /// </summary>
    public static GenderEnum Gender
    {
        get
        {
            if (!_genderLoaded)
                _gender = (GameBase.Strings.GetValue("Gender", "Male") == "Male") ? GenderEnum.MALE : GenderEnum.FEMALE;
            return _gender;
        }
        set
        {
            _gender = value;
            GameBase.Strings.SetValue("Gender", (value == GenderEnum.MALE) ? "Male" : "Female");
            GameBase.Strings.Save();
        }
    }
    /// <summary>
    /// DO NOT REFERENCE DIRECTLY. Use Gender instead.
    /// </summary>
    private static GenderEnum _gender;
    /// <summary>
    /// Flag for loading gender
    /// </summary>
    private static bool _genderLoaded = false;
    #endregion

    private const int interviewPassedCutoff = 400;
    public static int PassingCuttoff
    {
        get { return interviewPassedCutoff; }
    }
}