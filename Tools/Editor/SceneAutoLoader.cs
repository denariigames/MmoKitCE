#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

[InitializeOnLoad]
static class SceneAutoLoader
{
    private const string SettingsFilePath = "ProjectSettings/SceneAutoLoader.json";

    [System.Serializable]
    private class AutoLoaderSettings
    {
        public bool LoadMasterOnPlay = false;
        public string MasterScene = "Assets/MmoKitCE_addons/Demos/TinyEpicDemo/Scenes/TinyEpicDemo-Init.unity"; // default example
        public string PreviousScene = "";
    }

    private static AutoLoaderSettings settings;

    static SceneAutoLoader()
    {
        LoadSettings();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void LoadSettings()
    {
        if (File.Exists(SettingsFilePath))
        {
            string json = File.ReadAllText(SettingsFilePath);
            settings = JsonUtility.FromJson<AutoLoaderSettings>(json);
        }
        else
        {
            settings = new AutoLoaderSettings();
            SaveSettings();
        }
    }

    private static void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(SettingsFilePath, json);
    }

    // ====================== Menu Items ======================

    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Select Master Scene...")]
    private static void SelectMasterScene()
    {
        string path = EditorUtility.OpenFilePanel("Select Master Scene", Application.dataPath, "unity");
        if (!string.IsNullOrEmpty(path))
        {
            path = path.Replace(Application.dataPath, "Assets");
            settings.MasterScene = path;
            settings.LoadMasterOnPlay = true;
            SaveSettings();
        }
    }

    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Load Master On Play", true)]
    private static bool ShowLoadMasterOnPlay() => !settings.LoadMasterOnPlay;

    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Load Master On Play")]
    private static void EnableLoadMasterOnPlay()
    {
        settings.LoadMasterOnPlay = true;
        SaveSettings();
    }

    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Don't Load Master On Play", true)]
    private static bool ShowDontLoadMasterOnPlay() => settings.LoadMasterOnPlay;

    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Don't Load Master On Play")]
    private static void DisableLoadMasterOnPlay()
    {
        settings.LoadMasterOnPlay = false;
        SaveSettings();
    }

    // ====================== Play Mode Handling ======================

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!settings.LoadMasterOnPlay)
            return;

        if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // User pressed Play
            settings.PreviousScene = EditorSceneManager.GetActiveScene().path;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                try
                {
                    EditorSceneManager.OpenScene(settings.MasterScene);
                    SaveSettings(); // save previous scene
                }
                catch
                {
                    Debug.LogError($"[SceneAutoLoader] Master scene not found: {settings.MasterScene}");
                    EditorApplication.isPlaying = false;
                }
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }

        // User pressed Stop
        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            if (!string.IsNullOrEmpty(settings.PreviousScene))
            {
                try
                {
                    EditorSceneManager.OpenScene(settings.PreviousScene);
                }
                catch
                {
                    Debug.LogError($"[SceneAutoLoader] Previous scene not found: {settings.PreviousScene}");
                }
            }
        }
    }

    // Optional: Add a menu to show current settings
    [MenuItem("MMORPG KIT/MmoKitCE/Scene Autoload/Show Current Settings")]
    private static void ShowCurrentSettings()
    {
        Debug.Log($"[SceneAutoLoader] Load Master On Play: {settings.LoadMasterOnPlay}\n" +
                  $"Master Scene: {settings.MasterScene}\n" +
                  $"Previous Scene: {settings.PreviousScene}");
    }
}

#endif