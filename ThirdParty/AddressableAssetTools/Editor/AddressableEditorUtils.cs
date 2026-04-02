#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Insthync.AddressableAssetTools
{
    public static partial class AddressableEditorUtils
    {
        public static void CreateSettings()
        {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            }
        }

        public static AddressableAssetGroup CreateGroup(string name)
        {
            CreateSettings();

            AddressableAssetGroup addressableAssetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(name);

            if (addressableAssetGroup == null)
            {
                addressableAssetGroup = AddressableAssetSettingsDefaultObject.Settings.CreateGroup(name, false, false, false, new List<AddressableAssetGroupSchema>(), new System.Type[0]);
            }

            // Make sure we are using the default schemas with the default settings
            if (addressableAssetGroup != null)
            {
                addressableAssetGroup.RemoveSchema(typeof(BundledAssetGroupSchema));
                addressableAssetGroup.RemoveSchema(typeof(ContentUpdateGroupSchema));
                addressableAssetGroup.AddSchema(typeof(BundledAssetGroupSchema));
                addressableAssetGroup.AddSchema(typeof(ContentUpdateGroupSchema));
            }
            return addressableAssetGroup;
        }

        public static bool IsListOrArray(System.Type type, out System.Type itemType)
        {
            if (type.IsArray)
            {
                itemType = type.GetElementType();
                return true;
            }
            foreach (System.Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    itemType = type.GetGenericArguments()[0];
                    return true;
                }
            }
            itemType = null;
            return false;
        }

        public static void ConvertObjectRefToAddressable<TObject, TAssetRef>(ref TObject obj, ref TAssetRef aa, string groupName = "Default Local Group")
            where TObject : Object
            where TAssetRef : AssetReference
        {
            if (obj == null)
                return;
            if (string.IsNullOrWhiteSpace(groupName))
                groupName = "Default Local Group";
            FixMissingAssetReference(aa, groupName);
            string objPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrWhiteSpace(objPath))
            {
                Debug.LogWarning($"Skipping {obj.name}: Not an assets.");
                return;
            }
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }
            AddressableAssetGroup targetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
            if (targetGroup == null)
                targetGroup = CreateGroup(groupName);
            string guid = AssetDatabase.AssetPathToGUID(objPath);
            settings.CreateOrMoveEntry(guid, targetGroup, false, false);
            Debug.Log($"{obj.name} is moved to group: {groupName}.");
            object aaObject;
            if (typeof(TAssetRef) == typeof(AssetReferenceSprite))
            {
                Sprite spr = obj as Sprite;
                AssetReferenceSprite aaSpr = new AssetReferenceSprite(guid);
                aaSpr.SetEditorSubObject(spr);
                aaObject = aaSpr;
            }
            else if (typeof(TAssetRef) == typeof(AssetReferenceAudioClip))
            {
                AudioClip clip = obj as AudioClip;
                AssetReferenceAudioClip aaClip = new AssetReferenceAudioClip(clip);
                aaClip.SetEditorAsset(clip);
                aaObject = aaClip;
            }
            else
            {
                aaObject = System.Activator.CreateInstance(typeof(TAssetRef), new object[]
                {
                    guid,
                });
            }
            aa = (TAssetRef)aaObject;
            obj = null;
        }

        public static void ConvertObjectRefsToAddressables<TObject, TAssetRef>(ref IList<TObject> objs, ref List<TAssetRef> aas, string groupName = "Default Local Group")
            where TObject : Object
            where TAssetRef : AssetReference
        {
            if (objs == null || objs.Count <= 0)
                return;
            aas = new List<TAssetRef>();
            for (int i = 0; i < objs.Count; ++i)
            {
                TObject obj = objs[i];
                TAssetRef aa = null;
                ConvertObjectRefToAddressable(ref obj, ref aa, groupName);
                aas.Add(aa);
            }
            objs = null;
        }

        public static void ConvertObjectRefsToAddressables<TObject, TAssetRef>(ref IList<TObject> objs, ref TAssetRef[] aas, string groupName = "Default Local Group")
            where TObject : Object
            where TAssetRef : AssetReference
        {
            if (objs == null || objs.Count <= 0)
                return;
            aas = new TAssetRef[objs.Count];
            for (int i = 0; i < objs.Count; ++i)
            {
                TObject obj = objs[i];
                TAssetRef aa = null;
                ConvertObjectRefToAddressable(ref obj, ref aa, groupName);
                aas[i] = aa;
            }
            objs = null;
        }

        public static void ConvertObjectRefToAddressable(object source, System.Type sourceType, string objFieldName, string aaFieldName, bool clearSourceValue, string groupName = "Default Local Group")
        {
            if (source == null)
                return;
            if (string.IsNullOrWhiteSpace(groupName))
                groupName = "Default Local Group";
            FixMissingAssetReference(source, sourceType, aaFieldName, groupName);
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo objFieldInfo = sourceType.GetField(objFieldName, flags);
            FieldInfo aaFieldInfo = sourceType.GetField(aaFieldName, flags);
            if (objFieldInfo == null || objFieldInfo.GetValue(source) == null)
            {
                Debug.LogWarning($"Skipping object: {objFieldName} it is null.");
                return;
            }
            Object obj = objFieldInfo.GetValue(source) as Object;
            if (obj == null)
            {
                Debug.LogWarning($"Skipping object: {objFieldName} not an assets.");
                return;
            }
            string objPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrWhiteSpace(objPath))
            {
                Debug.LogWarning($"Skipping {obj.name}: Not an assets.");
                return;
            }
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }
            AddressableAssetGroup targetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
            if (targetGroup == null)
                targetGroup = CreateGroup(groupName);
            string guid = AssetDatabase.AssetPathToGUID(objPath);
            settings.CreateOrMoveEntry(guid, targetGroup, false, false);
            Debug.Log($"{obj.name} is moved to group: {groupName}.");
            object aaObject;
            if (aaFieldInfo.FieldType == typeof(AssetReferenceSprite))
            {
                Sprite spr = obj as Sprite;
                AssetReferenceSprite aaSpr = new AssetReferenceSprite(guid);
                aaSpr.SetEditorSubObject(spr);
                aaObject = aaSpr;
            }
            else if (aaFieldInfo.FieldType == typeof(AssetReferenceAudioClip))
            {
                AudioClip clip = obj as AudioClip;
                AssetReferenceAudioClip aaClip = new AssetReferenceAudioClip(clip);
                aaClip.SetEditorAsset(clip);
                aaObject = aaClip;
            }
            else
            {
                aaObject = System.Activator.CreateInstance(aaFieldInfo.FieldType, new object[]
                {
                    guid,
                });
            }
            aaFieldInfo.SetValue(source, aaObject);
            if (clearSourceValue)
                objFieldInfo.SetValue(source, null);
        }

        public static void ConvertObjectRefsToAddressables(object source, System.Type sourceType, string objsFieldName, string aasFieldName, bool clearSourceValue, string groupName = "Default Local Group")
        {
            if (source == null)
                return;
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo objsFieldInfo = sourceType.GetField(objsFieldName, flags);
            FieldInfo aasFieldInfo = sourceType.GetField(aasFieldName, flags);
            if (objsFieldInfo == null)
            {
                Debug.LogError($"Unable to find ref field {objsFieldName}");
                return;
            }
            if (aasFieldInfo == null)
            {
                Debug.LogError($"Unable to find addressable ref field {aasFieldName}");
                return;
            }
            if (!IsListOrArray(objsFieldInfo.FieldType, out System.Type objsItemType))
            {
                Debug.LogError($"Objects field named {objsFieldName} not a list or array.");
                return;
            }
            if (!IsListOrArray(aasFieldInfo.FieldType, out System.Type aasItemType))
            {
                Debug.LogError($"Addressable asset ref field named {aasFieldName} not a list or array.");
                return;
            }
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }
            AddressableAssetGroup targetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
            if (targetGroup == null)
                targetGroup = CreateGroup(groupName);
            IList objs = objsFieldInfo.GetValue(source) as IList;
            if (objs != null && objs.Count > 0)
            {
                System.Array aaArr = System.Array.CreateInstance(aasItemType, objs.Count);
                for (int i = 0; i < objs.Count; ++i)
                {
                    Object obj = objs[i] as Object;
                    if (obj == null)
                    {
                        Debug.LogWarning($"Skipping object: {objsFieldName}[{i}] not an assets.");
                        continue;
                    }
                    string objPath = AssetDatabase.GetAssetPath(obj);
                    if (string.IsNullOrWhiteSpace(objPath))
                    {
                        Debug.LogWarning($"Skipping {obj.name}: Not an assets.");
                        return;
                    }
                    string guid = AssetDatabase.AssetPathToGUID(objPath);
                    settings.CreateOrMoveEntry(guid, targetGroup, false, false);
                    Debug.Log($"{obj.name} is moved to group: {groupName}.");
                    object aaObject;
                    if (aasItemType == typeof(AssetReferenceSprite))
                    {
                        Sprite spr = obj as Sprite;
                        AssetReferenceSprite aaSpr = new AssetReferenceSprite(guid);
                        aaSpr.SetEditorSubObject(spr);
                        aaObject = aaSpr;
                    }
                    else if (aasItemType == typeof(AssetReferenceAudioClip))
                    {
                        AudioClip clip = obj as AudioClip;
                        AssetReferenceAudioClip aaClip = new AssetReferenceAudioClip(clip);
                        aaClip.SetEditorAsset(clip);
                        aaObject = aaClip;
                    }
                    else
                    {
                        aaObject = System.Activator.CreateInstance(aasItemType, new object[]
                        {
                            guid,
                        });
                    }
                    aaArr.SetValue(aaObject, i);
                }
                if (aasFieldInfo.FieldType.IsArray)
                {
                    aasFieldInfo.SetValue(source, aaArr);
                }
                else
                {
                    // NOTE: Just guess that it is a list
                    aasFieldInfo.SetValue(source, System.Activator.CreateInstance(aasFieldInfo.FieldType, new object[]
                    {
                        aaArr
                    }));
                }
                if (clearSourceValue)
                    objsFieldInfo.SetValue(source, null);
            }
        }

        public static void FixMissingAssetReference(object source, System.Type sourceType, string aaFieldName, string groupName = "Default Local Group")
        {
            if (source == null)
            {
                Debug.LogError("Unable to get source value.");
                return;
            }
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            FieldInfo aaFieldInfo = sourceType.GetField(aaFieldName, flags);
            AssetReference aaVar;
            try
            {
                aaVar = (AssetReference)aaFieldInfo.GetValue(source);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Unable to get value from {source} -> {aaFieldName}, {ex.Message}");
                return;
            }
            FixMissingAssetReference(aaVar, groupName);
        }

        public static void FixMissingAssetReference(AssetReference aaVar, string groupName = "Default Local Group")
        {
            if (aaVar == null)
            {
                Debug.LogError("Unable to get asset reference value.");
                return;
            }
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("Addressable Asset Settings not found!", MessageType.Error);
                return;
            }
            AddressableAssetGroup targetGroup = AddressableAssetSettingsDefaultObject.Settings.FindGroup(groupName);
            if (targetGroup == null)
                targetGroup = CreateGroup(groupName);
            string guid = aaVar.AssetGUID;
            if (string.IsNullOrWhiteSpace(guid))
            {
                Debug.Log("Asset GUID is null");
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.Log("Asset path is null");
                return;
            }
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry != null)
            {
                groupName = entry.parentGroup.Name;
                targetGroup = entry.parentGroup;
                Debug.Log($"(FIX) Asset: {guid} from {path} is in group: {groupName}.");
            }
            settings.CreateOrMoveEntry(guid, targetGroup, false, false);
            Debug.Log($"(FIX) Asset: {guid} from {path} is moved to group: {groupName}.");
            if (aaVar is AssetReferenceSprite aaSpr)
            {
                Sprite spr = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                aaSpr.SetEditorSubObject(spr);
            }
            else if (aaVar is AssetReferenceAudioClip aaAc)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                aaAc.SetEditorAsset(clip);
            }
        }
    }
}
#endif