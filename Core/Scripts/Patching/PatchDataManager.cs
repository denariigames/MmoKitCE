using Insthync.AddressableAssetTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace MultiplayerARPG
{
    public static class PatchDataManager
    {
        public const string KEY_PATCHING = "__PATCHING";
        public const string KEY_TYPE = "__TYPE";
        public const string KEY_ID = "__ID";
        public const string KEY_KEY = "__KEY";

        /// <summary>
        /// Dictionary: [$"{TYPE}_{ID}, DATA]
        /// </summary>
        public static readonly Dictionary<string, IPatchableData> PatchableData = new Dictionary<string, IPatchableData>();
        /// <summary>
        /// Dictionary: [$"{TYPE}_{ID}, DATA]
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, object>> PatchingData = new Dictionary<string, Dictionary<string, object>>();

        public static string GetPatchKey(this IPatchableData patchableData)
        {
            return $"{patchableData.GetType().FullName}_{patchableData.Id}";
        }

        public static Dictionary<string, object> GetExportDataForPatching(this IPatchableData target)
        {
            if (target == null)
                return null;
            Dictionary<string, object> result = GetExportData(target);
            result[KEY_TYPE] = target.GetType().FullName;
            result[KEY_ID] = target.Id;
            return result;
        }

        public static Dictionary<string, object> GetExportData(this object target)
        {
            if (target == null)
                return null;
            Dictionary<string, object> result = new Dictionary<string, object>();
            List<FieldInfo> exportingFields = new List<FieldInfo>(target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            for (int i = exportingFields.Count - 1; i >= 0; --i)
            {
                bool isRemoving = false;
                if (!exportingFields[i].IsPublic && !exportingFields[i].HasAttribute<SerializeField>())
                {
                    isRemoving = true;
                }
                if (exportingFields[i].HasAttribute<NonSerializedAttribute>())
                {
                    isRemoving = true;
                }
                if (isRemoving)
                {
                    exportingFields.RemoveAt(i);
                }
            }
            foreach (FieldInfo field in exportingFields)
            {
                Type fieldType = field.FieldType;
                if (fieldType.IsListOrArray(out Type elementType))
                {
                    if (elementType.IsClass)
                    {
                        if (elementType == typeof(string))
                        {
                            result[field.Name] = field.GetValue(target);
                            continue;
                        }
                        IList arr = field.GetValue(target) as IList;
                        if (arr == null)
                        {
                            continue;
                        }
                        List<Dictionary<string, object>> dicts = new List<Dictionary<string, object>>();
                        if (elementType.HasInterface<IGameData>())
                        {
                            for (int i = 0; i < arr.Count; ++i)
                            {
                                StoreGameDataPatchData(dicts, arr[i] as IGameData);
                            }
                            result[field.Name] = dicts;
                            continue;
                        }
#if !DISABLE_ADDRESSABLES
                        if (elementType.IsSubclassOf(typeof(AssetReference)))
                        {
                            for (int i = 0; i < arr.Count; ++i)
                            {
                                StoreAddressablePatchData(dicts, arr[i] as AssetReference);
                            }
                            result[field.Name] = dicts;
                            continue;
                        }
#endif
                        if (elementType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                            elementType.IsSubclassOf(typeof(Delegate)))
                        {
                            continue;
                        }
                        for (int i = 0; i < arr.Count; ++i)
                        {
                            dicts.Add(GetExportData(arr[i]));
                        }
                        result[field.Name] = dicts;
                    }
                    else if (elementType.IsValueType && !elementType.IsPrimitive && !elementType.IsEnum)
                    {
                        IList arr = field.GetValue(target) as IList;
                        if (arr == null)
                        {
                            continue;
                        }
                        List<Dictionary<string, object>> dicts = new List<Dictionary<string, object>>();
                        for (int i = 0; i < arr.Count; ++i)
                        {
                            dicts.Add(GetExportData(arr[i]));
                        }
                        result[field.Name] = dicts;
                    }
                    else
                    {
                        result[field.Name] = field.GetValue(target);
                    }
                }
                else if (fieldType.IsClass)
                {
                    if (fieldType == typeof(string))
                    {
                        result[field.Name] = field.GetValue(target);
                        continue;
                    }
                    if (fieldType.HasInterface<IGameData>())
                    {
                        StoreGameDataPatchData(result, field.Name, field.GetValue(target) as IGameData);
                        continue;
                    }
#if !DISABLE_ADDRESSABLES
                    if (fieldType.IsSubclassOf(typeof(AssetReference)))
                    {
                        StoreAddressablePatchData(result, field.Name, field.GetValue(target) as AssetReference);
                        continue;
                    }
#endif
                    if (fieldType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                        fieldType.IsSubclassOf(typeof(Delegate)))
                    {
                        continue;
                    }
                    result[field.Name] = GetExportData(field.GetValue(target));
                }
                else if (fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum)
                {
                    result[field.Name] = GetExportData(field.GetValue(target));
                }
                else
                {
                    result[field.Name] = field.GetValue(target);
                }
            }
            result[KEY_PATCHING] = true;
            return result;
        }

        private static void StoreGameDataPatchData(List<Dictionary<string, object>> list, IGameData gameData)
        {
            if (gameData == null || string.IsNullOrEmpty(gameData.Id))
                return;
            list.Add(GetGameDataPatchData(gameData));
        }

#if !DISABLE_ADDRESSABLES
        private static void StoreAddressablePatchData(List<Dictionary<string, object>> list, AssetReference aa)
        {
            if (!aa.IsDataValid())
                return;
            list.Add(GetAddressablePatchData(aa));
        }
#endif

        private static void StoreGameDataPatchData(Dictionary<string, object> result, string fieldName, IGameData gameData)
        {
            if (gameData == null || string.IsNullOrEmpty(gameData.Id))
                return;
            result[fieldName] = GetGameDataPatchData(gameData);
        }

#if !DISABLE_ADDRESSABLES
        private static void StoreAddressablePatchData(Dictionary<string, object> result, string fieldName, AssetReference aa)
        {
            if (!aa.IsDataValid())
                return;
            result[fieldName] = GetAddressablePatchData(aa);
        }
#endif

        private static Dictionary<string, object> GetGameDataPatchData(IGameData gameData)
        {
            if (gameData == null || string.IsNullOrEmpty(gameData.Id))
                return null;
            return new Dictionary<string, object>()
            {
                { KEY_TYPE, gameData.GetType().FullName },
                { KEY_ID,  gameData.Id },
            };
        }

#if !DISABLE_ADDRESSABLES
        private static Dictionary<string, object> GetAddressablePatchData(AssetReference aa)
        {
            if (!aa.IsDataValid())
                return null;
            return new Dictionary<string, object>()
            {
                { KEY_TYPE, aa.GetType().FullName },
                { KEY_KEY, aa.RuntimeKey },
            };
        }
#endif

        public static object ApplyPatch(this object target, Dictionary<string, object> patchingData)
        {
            if (target == null || patchingData == null || !patchingData.ContainsKey(KEY_PATCHING))
                return target;
            Type targetType = target.GetType();
            foreach (var entry in patchingData)
            {
                string fieldName = entry.Key;
                FieldInfo field = targetType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null)
                    continue;
                Type fieldType = field.FieldType;
                if (fieldType.IsListOrArray(out Type elementType))
                {
                    if (elementType.IsClass)
                    {
                        if (elementType == typeof(string))
                        {
                            field.SetValue(target, entry.Value);
                            continue;
                        }
                        List<Dictionary<string, object>> patchDataList = entry.Value as List<Dictionary<string, object>>;
                        if (patchDataList == null)
                        {
                            continue;
                        }
                        if (elementType.HasInterface<IGameData>())
                        {
                            Array dataArray = Array.CreateInstance(elementType, patchDataList.Count);
                            for (int i = 0; i < patchDataList.Count; ++i)
                            {
                                Dictionary<string, object> patchData = patchDataList[i];
                                if (!patchData.TryGetValue(KEY_TYPE, out object type) || !patchData.TryGetValue(KEY_ID, out object id))
                                    continue;
                                // TODO: Get data by type and id
                                IGameData foundData = null;
                                dataArray.SetValue(foundData, i);
                            }
                            ApplyListOrArrayField(dataArray, field, target);
                            continue;
                        }
#if !DISABLE_ADDRESSABLES
                        if (elementType.IsSubclassOf(typeof(AssetReference)))
                        {
                            Array dataArray = Array.CreateInstance(elementType, patchDataList.Count);
                            for (int i = 0; i < patchDataList.Count; ++i)
                            {
                                Dictionary<string, object> patchData = patchDataList[i];
                                if (!patchData.TryGetValue(KEY_KEY, out object aaKey))
                                    continue;
                                ApplyAddressablePatchData(Activator.CreateInstance(elementType) as AssetReference, aaKey);
                            }
                            ApplyListOrArrayField(dataArray, field, target);
                            continue;
                        }
#endif
                        if (elementType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                            elementType.IsSubclassOf(typeof(Delegate)))
                        {
                            continue;
                        }
                        ApplyListOfPatchData(patchDataList, field, elementType, target);
                    }
                    else if (elementType.IsValueType && !elementType.IsPrimitive && !elementType.IsEnum)
                    {
                        List<Dictionary<string, object>> patchDataList = entry.Value as List<Dictionary<string, object>>;
                        if (patchDataList == null)
                        {
                            continue;
                        }
                        ApplyListOfPatchData(patchDataList, field, elementType, target);
                    }
                    else
                    {
                        field.SetValue(target, entry.Value);
                    }
                }
                else if (fieldType.IsClass)
                {
                    if (fieldType == typeof(string))
                    {
                        field.SetValue(target, entry.Value);
                        continue;
                    }
                    if (fieldType.HasInterface<IGameData>())
                    {
                        ApplyGameDataPatchData(field, target, entry.Value as Dictionary<string, object>);
                        continue;
                    }
#if !DISABLE_ADDRESSABLES
                    if (fieldType.IsSubclassOf(typeof(AssetReference)))
                    {
                        ApplyAddressablePatchData(field, target, entry.Value as Dictionary<string, object>);
                        continue;
                    }
#endif
                    if (fieldType.IsSubclassOf(typeof(UnityEngine.Object)) ||
                        fieldType.IsSubclassOf(typeof(Delegate)))
                    {
                        continue;
                    }
                    field.SetValue(target, ApplyPatch(field.GetValue(target), entry.Value as Dictionary<string, object>));
                }
                else if (fieldType.IsValueType && !fieldType.IsPrimitive && !fieldType.IsEnum)
                {
                    field.SetValue(target, ApplyPatch(field.GetValue(target), entry.Value as Dictionary<string, object>));
                }
                else
                {
                    field.SetValue(target, entry.Value);
                }
            }
            return target;
        }

        private static void ApplyListOrArrayField(Array dataArray, FieldInfo field, object target)
        {
            if (field.FieldType.IsArray)
            {
                field.SetValue(target, dataArray);
            }
            else
            {
                IList list = field.GetValue(target) as IList;
                list.Clear();
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    list.Add(dataArray.GetValue(i));
                }
            }
        }

        private static void ApplyListOfPatchData(List<Dictionary<string, object>> patchDataList, FieldInfo field, Type elementType, object target)
        {
            Array dataArray = Array.CreateInstance(elementType, patchDataList.Count);
            for (int i = 0; i < patchDataList.Count; ++i)
            {
                dataArray.SetValue(ApplyPatch(Activator.CreateInstance(elementType), patchDataList[i]), i);
            }
            ApplyListOrArrayField(dataArray, field, target);
        }

        private static void ApplyGameDataPatchData(FieldInfo field, object target, Dictionary<string, object> patchData)
        {
            if (field.DeclaringType is not IGameData)
                return;
            if (!patchData.TryGetValue(KEY_TYPE, out object type) || !patchData.TryGetValue(KEY_ID, out object id))
                return;
            // TODO: Get data by type and id
            IGameData foundData = null;
            field.SetValue(target, foundData);
        }

#if !DISABLE_ADDRESSABLES
        private static void ApplyAddressablePatchData(FieldInfo field, object target, Dictionary<string, object> patchData)
        {
            AssetReference aa = field.GetValue(target) as AssetReference;
            if (aa == null || patchData == null)
                return;
            if (!patchData.TryGetValue(KEY_KEY, out object aaKey))
                return;
            ApplyAddressablePatchData(aa, aaKey);
        }
#endif

#if !DISABLE_ADDRESSABLES
        private static void ApplyAddressablePatchData(AssetReference aa, object guid)
        {
            FieldInfo guidField = aa.GetType().GetField("m_AssetGUID", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            guidField.SetValue(aa, guid);
        }
#endif
    }
}
