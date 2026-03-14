using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public class PlayerPrefsTools
    {
        [MenuItem(EditorMenuConsts.DELETE_ALL_PLAYER_PREFS_MENU, false, EditorMenuConsts.DELETE_ALL_PLAYER_PREFS_ORDER)]
        public static void DeleteAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
