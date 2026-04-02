#if !DISABLE_ADDRESSABLES
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System.Reflection;
using System.Collections;

namespace Insthync.AddressableAssetTools
{
    public class SetProperAddressableNameEditor : EditorWindow
    {
        private List<Object> _selectedAssets = new List<Object>();
        private Vector2 _assetsScrollPosition;

        [MenuItem("Tools/Addressables/Set Proper Addressable Name")]
        public static void ShowWindow()
        {
            GetWindow<SetProperAddressableNameEditor>("Set Proper Addressable Name");
        }

        private void OnGUI()
        {
            GUILayout.Label("Set Proper Addressable Name", EditorStyles.boldLabel);

            GUILayout.Label("Selected Assets:", EditorStyles.boldLabel);

            // Scrollable list of selected assets
            _assetsScrollPosition = EditorGUILayout.BeginScrollView(_assetsScrollPosition, GUILayout.Height(150));
            for (int i = 0; i < _selectedAssets.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _selectedAssets[i] = EditorGUILayout.ObjectField(_selectedAssets[i], typeof(Object), false);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    _selectedAssets.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Asset"))
            {
                _selectedAssets.Add(null);
            }

            if (GUILayout.Button("Add Selected Assets (In Project Tab)"))
            {
                _selectedAssets.AddRange(Selection.objects);
            }

            if (GUILayout.Button("Clear Assets"))
            {
                _selectedAssets.Clear();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Set Proper Name"))
            {
                SetProperName();
            }
        }

        private void SetProperName()
        {
            for (int i = 0; i < _selectedAssets.Count; ++i)
            {
                SetProperName(_selectedAssets[i]);
            }
            AssetDatabase.SaveAssets();
        }

        private void SetProperName(Object asset)
        {
            if (asset == null)
                return;
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string oldAssetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            string assetName = oldAssetName;
            assetName = assetName.Replace("[", "(").Replace("]", ")");
            if (!string.Equals(assetName, oldAssetName))
            {
                string newAssetPath = assetPath.Replace(oldAssetName, assetName);
                if (AssetDatabase.DeleteAsset(newAssetPath))
                    Debug.Log($"Delete: {newAssetPath}");
                string error = AssetDatabase.RenameAsset(assetPath, assetName);
                if (!string.IsNullOrWhiteSpace(error))
                    Debug.LogError(error);
            }
        }
    }
}
#endif