#if !DISABLE_ADDRESSABLES
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Insthync.AddressableAssetTools
{
    public abstract class BaseAddAddressableToGroupEditor : EditorWindow
    {
        protected AddressableAssetSettings _settings;
        protected AddressableAssetGroup _selectedGroup;
        protected AddressableAssetGroup _dirtySelectedGroup;
        protected List<Object> _selectedAssets = new List<Object>();
        protected List<string> _dependencyPaths = new List<string>();
        protected Dictionary<string, bool> _dependencySelection = new Dictionary<string, bool>();
        protected Vector2 _assetsScrollPosition;
        protected bool _excludeFromOtherGroups = true;
        protected Vector2 _dependenciesScrollPosition;

        protected virtual void OnGUI_AddDependenciesToGroupSection() { }
        protected virtual void OnGUI_SelectedAssetsSection() { }

        protected virtual void OnGUI()
        {
            GUILayout.Label("Add Dependencies to Group", EditorStyles.boldLabel);

            _settings = AddressableAssetSettingsDefaultObject.Settings;
            if (_settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }
            _selectedGroup = (AddressableAssetGroup)EditorGUILayout.ObjectField("Target Group", _selectedGroup, typeof(AddressableAssetGroup), false);
            if (_dirtySelectedGroup != _selectedGroup)
            {
                _dirtySelectedGroup = _selectedGroup;
                _selectedAssets.Clear();
                if (_selectedGroup != null)
                {
                    var entries = _selectedGroup.entries;
                    foreach (var entry in entries)
                    {
                        _selectedAssets.Add(entry.TargetAsset);
                    }
                }
            }
            EditorGUILayout.Space();

            OnGUI_AddDependenciesToGroupSection();
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

            _excludeFromOtherGroups = EditorGUILayout.Toggle("Exclude From Other Groups", _excludeFromOtherGroups);
            OnGUI_SelectedAssetsSection();

            if (GUILayout.Button("Find Dependencies of Selected Assets"))
            {
                FindDependencies();
            }

            if (_dependencyPaths.Count > 0)
            {
                GUILayout.Label("Select Dependencies to Add:", EditorStyles.boldLabel);

                // Begin Scroll View for Dependencies
                _dependenciesScrollPosition = EditorGUILayout.BeginScrollView(_dependenciesScrollPosition, GUILayout.Height(300));

                EditorGUILayout.BeginVertical("box");
                foreach (var dependencyPath in _dependencyPaths)
                {
                    _dependencySelection[dependencyPath] = EditorGUILayout.ToggleLeft(dependencyPath, _dependencySelection[dependencyPath]);
                }
                EditorGUILayout.EndVertical();

                // End Scroll View
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                // Select/Deselect All Buttons
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    SetAllDependenciesSelection(true);
                }
                if (GUILayout.Button("Deselect All"))
                {
                    SetAllDependenciesSelection(false);
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Select Dependencies In Project Tab"))
                {
                    SelectDependenciesInProjectTab();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Selected Dependencies to Group"))
                {
                    AddSelectedDependencies();
                }
            }
        }

        protected void FindDependencies()
        {
            _dependencyPaths.Clear();
            _dependencySelection.Clear();

            foreach (var asset in _selectedAssets)
            {
                if (asset == null) continue;

                string assetPath = AssetDatabase.GetAssetPath(asset);
                string[] dependencies = AssetDatabase.GetDependencies(assetPath, true);

                foreach (var dependencyPath in dependencies)
                {
                    // Exclude the asset itself, source code files, and dependencies already in another group
                    if (dependencyPath == assetPath || IsSourceCodeFile(dependencyPath))
                        continue;

                    if (_dependencyPaths.Contains(dependencyPath))
                        continue;

                    if (_excludeFromOtherGroups && IsInAnyAddressableGroup(dependencyPath))
                        continue;

                    if (!IsTargetAsset(dependencyPath))
                        continue;

                    var depGuid = AssetDatabase.GUIDFromAssetPath(dependencyPath);
                    Debug.Log($"Found {dependencyPath} ({depGuid}) is ref by {assetPath}", asset);
                    _dependencyPaths.Add(dependencyPath);
                    _dependencySelection[dependencyPath] = true; // Default to selected
                }
            }

            // Sort dependencies by path
            _dependencyPaths.Sort();
        }

        protected bool IsSourceCodeFile(string dependencyPath)
        {
            return dependencyPath.EndsWith(".cs") || dependencyPath.EndsWith(".js") || dependencyPath.EndsWith(".boo");
        }

        protected abstract bool IsTargetAsset(string dependencyPath);

        protected bool IsInAnyAddressableGroup(string dependencyPath)
        {
            string guid = AssetDatabase.AssetPathToGUID(dependencyPath);
            AddressableAssetEntry entry = _settings.FindAssetEntry(guid);

            // Return true if the asset is in a group
            return entry != null;
        }

        protected void AddSelectedDependencies()
        {
            if (_selectedGroup == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a target Addressable Group.", "OK");
                return;
            }

            foreach (var dependencyPath in _dependencyPaths)
            {
                if (_dependencySelection[dependencyPath])
                {
                    AddressableAssetEntry dependencyEntry = _settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(dependencyPath));
                    if (dependencyEntry == null)
                    {
                        _settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(dependencyPath), _selectedGroup, false, false);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Done", "Selected dependencies have been added to the group.", "OK");
        }

        protected void SetAllDependenciesSelection(bool isSelected)
        {
            foreach (var key in _dependencyPaths)
            {
                _dependencySelection[key] = isSelected;
            }
        }

        protected void SelectDependenciesInProjectTab()
        {
            List<Object> selectingObjects = new List<Object>();
            foreach (var kv in _dependencySelection)
            {
                if (kv.Value)
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(kv.Key);
                    if (obj != null)
                    {
                        selectingObjects.Add(obj);
                    }
                }
            }
            Selection.objects = selectingObjects.ToArray();
        }
    }
}
#endif