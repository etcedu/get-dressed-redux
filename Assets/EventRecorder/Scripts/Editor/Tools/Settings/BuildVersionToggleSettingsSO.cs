#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BuildVersionToggleSettingsSO : ScriptableObject
{
    public const string SettingsFilename = "BuildVersionToggleSettings"; //Default name of the auto created instance of this SO

    public string bundleID_Consumer = "com.simcoachgames.simcoachcarddeck";
    public string bundleID_Enterprise = "com.simcoachgames.simcoachcarddeckenterprise";

    public string productName_Consumer = "Simcoach Card Deck";
    public string productName_Enterprise = "Simcoach Card Deck Enterprise";

    public SceneAsset menuScene_Consumer;
    public SceneAsset menuScene_Enterprise;
    


    static BuildVersionToggleSettingsSO Instance;

    public static BuildVersionToggleSettingsSO GetInstance()
    {
        if (Instance != null)
            return Instance;

        Instance = Resources.Load<BuildVersionToggleSettingsSO>(string.Format("BuildVersionToggle/{0}", SettingsFilename));

#if UNITY_EDITOR
        if (!Instance)
        {
            if (!Directory.Exists("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!Directory.Exists("Assets/Resources/BuildVersionToggle"))
                AssetDatabase.CreateFolder("Assets/Resources", "BuildVersionToggle");

            AssetDatabase.CreateAsset(
               CreateInstance<BuildVersionToggleSettingsSO>(),
               string.Format("Assets/Resources/BuildVersionToggle/{0}.asset", SettingsFilename));

            Instance = Resources.Load<BuildVersionToggleSettingsSO>(string.Format("BuildVersionToggle/{0}", SettingsFilename));
        }
#endif

        if (!Instance)
        {
            Debug.LogError("Could not find or create Build Version Toggle Settings File");
        }

        return Instance;
    }

    public void SaveData()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }
}
