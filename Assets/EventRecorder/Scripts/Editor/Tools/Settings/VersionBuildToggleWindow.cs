using UnityEngine;
using UnityEditor;
using SimcoachGames.EventRecorder;

public class VersionBuildToggleWindow : EditorWindow
{
    static BuildVersionToggleSettingsSO buildVersionSettings;

    [MenuItem("Tools/Event Recorder/Consumer or Enterprise Version Toggle")]
    public static void ShowWindow()
    {
        VersionBuildToggleWindow window =
                (VersionBuildToggleWindow)GetWindow(typeof(VersionBuildToggleWindow), false,
                    "Build Version Toggle Settings");

        buildVersionSettings = BuildVersionToggleSettingsSO.GetInstance();
        if (!buildVersionSettings)
            return;
                
        window.Show();
    }

    void OnGUI()
    {
        if (!buildVersionSettings)
            buildVersionSettings = BuildVersionToggleSettingsSO.GetInstance();

        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        centeredStyle.richText = true;

        GUILayout.Label("<b>Enterprise Edition Toggle</b>", centeredStyle);
        GUILayout.Space(8);
        GUILayout.Label(string.Format("Current Mode: {0}", Application.identifier == buildVersionSettings.bundleID_Enterprise ? "<color=#FF0000>Enterprise</color>" : "<color=#00FFFF>Consumer</color>"), centeredStyle);
        GUILayout.Space(20);

        buildVersionSettings.bundleID_Consumer = EditorGUILayout.TextField("Default Bundle ID: ", buildVersionSettings.bundleID_Consumer);
        buildVersionSettings.bundleID_Enterprise = EditorGUILayout.TextField("Enterprise Bundle ID: ", buildVersionSettings.bundleID_Enterprise);
        buildVersionSettings.productName_Consumer = EditorGUILayout.TextField("Consumer Product Name: ", buildVersionSettings.productName_Consumer);
        buildVersionSettings.productName_Enterprise = EditorGUILayout.TextField("Enterprise Product Name: ", buildVersionSettings.productName_Enterprise);

        GUILayout.Space(4);

        GUILayout.Label("Consumer Menu Scene:", EditorStyles.boldLabel);
        buildVersionSettings.menuScene_Consumer = (SceneAsset)EditorGUILayout.ObjectField(buildVersionSettings.menuScene_Consumer, typeof(SceneAsset), false);
        GUILayout.Label("Enterprise Menu Scene:", EditorStyles.boldLabel);
        buildVersionSettings.menuScene_Enterprise = (SceneAsset)EditorGUILayout.ObjectField(buildVersionSettings.menuScene_Enterprise, typeof(SceneAsset), false);

        GUILayout.Space(8);

        EditorUtility.SetDirty(buildVersionSettings);

        if (GUILayout.Button("Set Consumer Build Settings"))
        {
            SetEditorBuildSettingsScenes(false);
            SetBundleID(false);
            SetProductName(false);
            SetEventID();
            SaveSettings();
        }
        if (GUILayout.Button("Set Enterprise Build Settings"))
        {
            SetEditorBuildSettingsScenes(true);
            SetBundleID(true);
            SetProductName(true);
            SetEventID();
            SaveSettings();
        }
    }

    static void SaveSettings()
    {
        buildVersionSettings = BuildVersionToggleSettingsSO.GetInstance();
        if (!buildVersionSettings)
            return;

        buildVersionSettings.SaveData();

        FindObjectOfType<SimcoachLicenseManagementUI>().blockerText.text 
            = FindObjectOfType<SimcoachLicenseManagementUI>().blockerText.text.Replace("{$game}", BuildVersionToggleSettingsSO.GetInstance().productName_Consumer);
    }

    static void SetEditorBuildSettingsScenes(bool enterprise)
    {
        EditorBuildSettingsScene[] editorBuildSettingsScenes = EditorBuildSettings.scenes;

        string scenePath = AssetDatabase.GetAssetPath(enterprise ? buildVersionSettings.menuScene_Enterprise : buildVersionSettings.menuScene_Consumer);
        if (!string.IsNullOrEmpty(scenePath))
            editorBuildSettingsScenes[0] = new EditorBuildSettingsScene(scenePath, true);

        EditorBuildSettings.scenes = editorBuildSettingsScenes;
    }

    static void SetBundleID(bool enterprise)
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, enterprise ? buildVersionSettings.bundleID_Enterprise : buildVersionSettings.bundleID_Consumer);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, enterprise ? buildVersionSettings.bundleID_Enterprise : buildVersionSettings.bundleID_Consumer);
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, enterprise ? buildVersionSettings.bundleID_Enterprise : buildVersionSettings.bundleID_Consumer);
    }

    static void SetProductName(bool enterprise)
    {
        PlayerSettings.productName = enterprise ? buildVersionSettings.productName_Enterprise : buildVersionSettings.productName_Consumer;
    }

    static void SetEventID()
    {
        EventIdValidationWindow.Init();
    }
}
