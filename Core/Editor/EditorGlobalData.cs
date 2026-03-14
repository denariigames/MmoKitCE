using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    public static class EditorGlobalData
    {
        private static EditorSettings s_settingsInstance = null;
        public static EditorSettings SettingsInstance
        {
            get
            {
                if (s_settingsInstance == null)
                    s_settingsInstance = Resources.Load<EditorSettings>("EditorSettings");
                return s_settingsInstance;
            }
        }
        public static Scene? EditorScene { get; set; }
        public static GameDatabase WorkingDatabase
        {
            get => SettingsInstance.workingDatabase;
            set => SettingsInstance.workingDatabase = value;
        }
        public static string[] SocketEnhancerTypeTitles
        {
            get => SettingsInstance.socketEnhancerTypeTitles;
            set => SettingsInstance.socketEnhancerTypeTitles = value;
        }
    }
}
