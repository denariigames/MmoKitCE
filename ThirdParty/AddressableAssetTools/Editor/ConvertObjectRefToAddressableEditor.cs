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
    public class ConvertObjectRefToAddressableEditor : EditorWindow
    {
        private AddressableAssetSettings _settings;
        private AddressableAssetGroup _selectedGroup;
        private AddressableAssetGroup _dirtySelectedGroup;
        private string _groupName = string.Empty;
        private bool _clearSourceValue = true;
        private List<Object> _selectedAssets = new List<Object>();
        private Vector2 _assetsScrollPosition;

        [MenuItem("Tools/Addressables/Convert Object Ref To Addressable")]
        public static void ShowWindow()
        {
            GetWindow<ConvertObjectRefToAddressableEditor>("Convert Object Ref To Addressable");
        }

        private void OnGUI()
        {
            GUILayout.Label("Convert Object Ref To Addressable", EditorStyles.boldLabel);

            _settings = AddressableAssetSettingsDefaultObject.Settings;
            if (_settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(_groupName))
            {
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
            }
            _groupName = EditorGUILayout.TextField("Target Group Name", _groupName);
            EditorGUILayout.Space();

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
            _clearSourceValue = EditorGUILayout.Toggle("Clear Source Value", _clearSourceValue);

            if (GUILayout.Button("Convert"))
            {
                ConvertSelectedAssets();
            }
        }

        private void ConvertSelectedAssets()
        {
            for (int i = 0; i < _selectedAssets.Count; ++i)
            {
                Convert(_selectedAssets[i]);
            }
        }

        private void Convert(Object asset)
        {
            if (asset == null)
                return;
            if (string.IsNullOrWhiteSpace(_groupName) && _selectedGroup != null)
                _groupName = _selectedGroup.Name;
            System.Type objectType = asset.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            do
            {
                List<FieldInfo> fields = new List<FieldInfo>(objectType.GetFields(flags));
                foreach (FieldInfo field in fields)
                {
                    System.Type tempElementType;
                    object[] foundAttr = field.GetCustomAttributes(typeof(AddressableAssetConversionAttribute), false);
                    if (foundAttr.Length > 0)
                    {
                        AddressableAssetConversionAttribute attr = foundAttr[0] as AddressableAssetConversionAttribute;
                        if (AddressableEditorUtils.IsListOrArray(field.FieldType, out tempElementType))
                        {
                            AddressableEditorUtils.ConvertObjectRefsToAddressables(asset, objectType, field.Name, attr.AddressableVarName, _clearSourceValue, _groupName);
                        }
                        else
                        {
                            AddressableEditorUtils.ConvertObjectRefToAddressable(asset, objectType, field.Name, attr.AddressableVarName, _clearSourceValue, _groupName);
                        }
                        continue;
                    }
                    System.Type interfaceType = field.FieldType.GetInterface(nameof(IAddressableAssetConversable));
                    if (interfaceType != null)
                    {
                        MethodInfo methodInfo = interfaceType.GetMethod("ProceedAddressableAssetConversion", flags);
                        methodInfo.Invoke(field.GetValue(asset), new object[]
                        {
                            _groupName
                        });
                        continue;
                    }
                    if (AddressableEditorUtils.IsListOrArray(field.FieldType, out tempElementType))
                    {
                        interfaceType = tempElementType.GetInterface(nameof(IAddressableAssetConversable));
                        if (interfaceType != null)
                        {
                            IList list = field.GetValue(asset) as IList;
                            if (list != null && list.Count > 0)
                            {
                                for (int i = 0; i < list.Count; ++i)
                                {
                                    object entry = list[i];
                                    MethodInfo methodInfo = interfaceType.GetMethod("ProceedAddressableAssetConversion", flags);
                                    methodInfo.Invoke(entry, new object[]
                                    {
                                    _groupName
                                    });
                                    list[i] = entry;
                                }
                            }
                        }
                    }
                }
                objectType = objectType.BaseType;
            } while (objectType.BaseType != null);
            EditorUtility.SetDirty(asset);
        }
    }
}
#endif