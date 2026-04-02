#if !DISABLE_ADDRESSABLES
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Insthync.AddressableAssetTools
{
    public class AddressablesCheckDuplicateGuids
    {
        [MenuItem("Tools/Addressables/Check Duplicate GUIDs")]
        public static void CheckDuplicateGuids()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var guidMap = new Dictionary<string, List<(string address, string name, string path)>>();

            foreach (var group in settings.groups)
            {
                foreach (var entry in group.entries)
                {
                    string path = entry.AssetPath;
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    string name = AssetDatabase.LoadAssetAtPath<Object>(path)?.name ?? "(Missing)";
                    string address = entry.address;

                    if (!guidMap.ContainsKey(guid))
                        guidMap[guid] = new List<(string, string, string)>();

                    guidMap[guid].Add((address, name, path));
                }
            }

            int duplicateCount = 0;
            foreach (var kv in guidMap.Where(kv => kv.Value.Count > 1))
            {
                duplicateCount++;
                Debug.LogError(
                    $"Duplicate GUID: {kv.Key}\n" +
                    $"{string.Join("\n", kv.Value.Select(v => $"- Name: {v.name} | Address: {v.address} | Path: {v.path}"))}"
                );
            }

            if (duplicateCount == 0)
                Debug.Log("No duplicate GUIDs found.");
            else
                Debug.LogWarning($"Found {duplicateCount} duplicate GUID(s).");
        }
    }
}
#endif