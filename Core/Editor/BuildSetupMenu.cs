using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace MultiplayerARPG
{
    public class BuildSettingMenu
    {
        [MenuItem(EditorMenuConsts.BUILD_SETUP_OFFLINE_LAN_MENU, false, EditorMenuConsts.BUILD_SETUP_OFFLINE_LAN_ORDER)]
        public static void BuildSetupOfflineLan()
        {
            RemoveFromDefines("EXCLUDE_SERVER_CODES");
            RemoveFromDefines("UNITY_SERVER");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for Offline/LAN games is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_MMO_MENU, false, EditorMenuConsts.BUILD_SETUP_MMO_ORDER)]
        public static void BuildSetupMMO()
        {
            AddToDefines("EXCLUDE_SERVER_CODES");
            RemoveFromDefines("UNITY_SERVER");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for MMO games is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_MMO_SERVER_INCLUDE_MENU, false, EditorMenuConsts.BUILD_SETUP_MMO_SERVER_INCLUDE_ORDER)]
        public static void BuildSetupMMOServerInclude()
        {
            RemoveFromDefines("EXCLUDE_SERVER_CODES");
            RemoveFromDefines("UNITY_SERVER");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for MMO (with Server Codes) is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_EXCLUDE_PREFAB_REFS_MENU, false, EditorMenuConsts.BUILD_SETUP_EXCLUDE_PREFAB_REFS_ORDER)]
        public static void BuildSetupExcludePrefabRefs()
        {
            AddToDefines("EXCLUDE_PREFAB_REFS");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for prefab refs excluding is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_INCLUDE_PREFAB_REFS_MENU, false, EditorMenuConsts.BUILD_SETUP_INCLUDE_PREFAB_REFS_ORDER)]
        public static void BuildSetupIncludePrefabRefs()
        {
            RemoveFromDefines("EXCLUDE_PREFAB_REFS");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for prefab refs including is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_DISABLE_ADDRESSABLES_MENU, false, EditorMenuConsts.BUILD_SETUP_DISABLE_ADDRESSABLES_ORDER)]
        public static void BuildSetupDisableAddressables()
        {
            AddToDefines("DISABLE_ADDRESSABLES");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for addressables disabling is done, you will have wait a bit for compiling", "Ok");
        }

        [MenuItem(EditorMenuConsts.BUILD_SETUP_ENABLE_ADDRESSABLES_MENU, false, EditorMenuConsts.BUILD_SETUP_ENABLE_ADDRESSABLES_ORDER)]
        public static void BuildSetupEnaableAddressables()
        {
            RemoveFromDefines("DISABLE_ADDRESSABLES");
            EditorUtility.DisplayDialog("Scripting Define Symbols Setup", "Scripting Define Symbols setup for addressables enabling is done, you will have wait a bit for compiling", "Ok");
        }

        private static void AddToDefines(string symbol)
        {
            string previousProjectDefines = GetCurrentProjectDefines(out BuildTargetGroup buildTargetGroup);
            List<string> projectDefines = previousProjectDefines.Split(';').ToList();
            if (!projectDefines.Contains(symbol))
                projectDefines.Add(symbol);
            string newDefines = string.Join(";", projectDefines.ToArray());
            if (previousProjectDefines != newDefines)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);
        }

        public static void RemoveFromDefines(string symbol)
        {
            string previousProjectDefines = GetCurrentProjectDefines(out BuildTargetGroup buildTargetGroup);
            List<string> projectDefines = previousProjectDefines.Split(';').ToList();
            if (projectDefines.Contains(symbol))
                projectDefines.Remove(symbol);
            string newDefines = string.Join(";", projectDefines.ToArray());
            if (previousProjectDefines != newDefines)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, newDefines);
        }

        private static string GetCurrentProjectDefines(out BuildTargetGroup buildTargetGroup)
        {
            buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                PropertyInfo propertyInfo = typeof(EditorUserBuildSettings).GetProperty("activeBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);
                if (propertyInfo != null)
                    buildTargetGroup = (BuildTargetGroup)propertyInfo.GetValue(null, null);
            }
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        }
    }
}
