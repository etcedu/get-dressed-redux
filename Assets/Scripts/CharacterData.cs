using UnityEngine;

[CreateAssetMenu(menuName = "Custom/CharacterData", fileName = "New Character Data")]
[System.Serializable]
public class CharacterData : ScriptableObject
{
    public string characterTag;
    public string characterName;
    public string jobTitle;
    public string description;
    public string jobAttireDescription;
    public string imageAssetPath;
    public string[] headOptions;
    public string[] topOptions;
    public string[] bottomOptions;
    public string[] feetOptions;
}
