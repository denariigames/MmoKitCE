#if !DISABLE_ADDRESSABLES
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Insthync.AddressableAssetTools
{
    public class AddressablesCleaner
    {
        [MenuItem("Tools/Addressables/Clean Missing Addressables")]
        public static void CleanMissingAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            foreach (var group in settings.groups)
            {
                var entries = new List<AddressableAssetEntry>(group.entries);
                for (int i = entries.Count - 1; i >= 0; --i)
                {
                    var entry = entries[i];
                    var assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
                    if (string.IsNullOrEmpty(assetPath) || AssetDatabase.LoadAssetAtPath<Object>(assetPath) == null)
                    {
                        Debug.Log($"Removed missing asset: {entry.address}");
                        group.RemoveAssetEntry(entry);
                    }
                }
            }

            AssetDatabase.SaveAssets();
        }
    }
}
#endif