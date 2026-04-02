#if !DISABLE_ADDRESSABLES
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Insthync.AddressableAssetTools
{
    public static class AddressableAssetsManager
    {
        private static readonly HashSet<object> s_loadingAssets = new HashSet<object>();
        private static readonly Dictionary<object, AsyncOperationHandle> s_loadedAssets = new Dictionary<object, AsyncOperationHandle>();
        private static readonly List<AsyncOperationHandle<SceneInstance>> s_addressableSceneHandles = new List<AsyncOperationHandle<SceneInstance>>();
        private static readonly Dictionary<object, IList<IResourceLocation>> s_resourceLocations = new Dictionary<object, IList<IResourceLocation>>();

        public static bool TryGetRuntimeKeyBySceneName(string sceneName, out object runtimeKey)
        {
            foreach (IResourceLocator locator in Addressables.ResourceLocators)
            {
                foreach (object key in locator.Keys)
                {
                    if (!locator.Locate(key, typeof(SceneInstance), out IList<IResourceLocation> list))
                        continue;
                    foreach (IResourceLocation location in list)
                    {
                        string path = Path.GetFileNameWithoutExtension(location.InternalId);
                        if (string.Equals(path, sceneName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            runtimeKey = key;
                            return true;
                        }
                    }
                }
            }
            runtimeKey = null;
            return false;
        }

        public static AsyncOperationHandle<SceneInstance> LoadAddressableScene(AssetReferenceScene addressableScene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            AsyncOperationHandle<SceneInstance> addressableAsyncOp = addressableScene.LoadSceneAsync(loadSceneMode, true);
            s_addressableSceneHandles.Add(addressableAsyncOp);
            return addressableAsyncOp;
        }

        public static void AddAddressableSceneHandle(AsyncOperationHandle<SceneInstance> addressableSceneHandle)
        {
            s_addressableSceneHandles.Add(addressableSceneHandle);
        }

        public static async UniTask UnloadAddressableScenes()
        {
            if (s_addressableSceneHandles.Count == 0)
                return;
            for (int i = 0; i < s_addressableSceneHandles.Count; ++i)
            {
                if (!s_addressableSceneHandles[i].IsValid())
                    continue;
                AsyncOperationHandle<SceneInstance> addressableAsyncOp = Addressables.UnloadSceneAsync(s_addressableSceneHandles[i], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, true);
                while (!addressableAsyncOp.IsDone)
                {
                    await UniTask.Yield();
                }
            }
            s_addressableSceneHandles.Clear();
        }

        public static string GetScenePathByRuntimeKey(object runtimeKey)
        {
            IResourceLocation resourceLocation = GetFirstResourceLocationByRuntimeKey(runtimeKey);
            if (resourceLocation == null)
                return string.Empty;
            return resourceLocation.InternalId;
        }

        public static string GetScenePath(this AssetReferenceScene scene)
        {
            return GetScenePathByRuntimeKey(scene.RuntimeKey);
        }

        public static string GetSceneName(this AssetReferenceScene scene)
        {
            return Path.GetFileNameWithoutExtension(scene.GetScenePath());
        }

        public static IList<IResourceLocation> GetResourceLocation(this AssetReference asset)
        {
            return GetResourceLocationByRuntimeKey(asset.RuntimeKey);
        }

        public static IList<IResourceLocation> GetResourceLocationByRuntimeKey(object runtimeKey)
        {
            if (s_resourceLocations.TryGetValue(runtimeKey, out IList<IResourceLocation> cachedLocations))
                return cachedLocations;
            foreach (IResourceLocator locator in Addressables.ResourceLocators)
            {
                if (!locator.Locate(runtimeKey, typeof(SceneInstance), out IList<IResourceLocation> list))
                    continue;
                s_resourceLocations[runtimeKey] = list;
                return list;
            }
            return null;
        }

        public static IResourceLocation GetFirstResourceLocation(this AssetReference asset)
        {
            return GetFirstResourceLocationByRuntimeKey(asset.RuntimeKey);
        }

        public static IResourceLocation GetFirstResourceLocationByRuntimeKey(object runtimeKey)
        {
            var list = GetResourceLocationByRuntimeKey(runtimeKey);
            if (list != null && list.Count > 0)
                return list[0];
            return null;
        }

        public static async UniTask<TType> GetOrLoadObjectAsync<TType>(this AssetReference assetRef)
            where TType : Object
        {
            object runtimeKey = assetRef.RuntimeKey;
            AsyncOperationHandle loadedHandle;
            if (s_loadedAssets.TryGetValue(runtimeKey, out loadedHandle))
                return loadedHandle.Result as TType;

            // Check if the asset is actually marked as Addressable
            if (!assetRef.IsDataValid())
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Asset is not marked as Addressable: {runtimeKey}. Ignoring load.");
#endif
                return null;
            }

            while (s_loadingAssets.Contains(runtimeKey))
            {
                await UniTask.Yield();
            }
            if (s_loadedAssets.TryGetValue(runtimeKey, out loadedHandle))
            {
                return loadedHandle.Result as TType;
            }
            s_loadingAssets.Add(runtimeKey);

            // Check if the Addressable asset exists before loading
            AsyncOperationHandle<IList<IResourceLocation>> loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync(runtimeKey);
            IList<IResourceLocation> locations = await loadResourceLocationsHandle.ToUniTask();
            loadResourceLocationsHandle.Release();
            if (locations == null || locations.Count == 0)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Addressable asset not found: {runtimeKey}. Ignoring load.");
#endif
                s_loadingAssets.Remove(runtimeKey);
                return null;
            }

            AsyncOperationHandle<TType> handler = Addressables.LoadAssetAsync<TType>(runtimeKey);
            try
            {
                handler = Addressables.LoadAssetAsync<TType>(runtimeKey);
                TType result = await handler.ToUniTask();
                s_loadedAssets[assetRef.RuntimeKey] = handler;
                s_loadingAssets.Remove(runtimeKey);
                return result;
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"Failed to load addressable asset asynchronously: {runtimeKey}, {ex.Message}\n{ex.StackTrace}");
#endif
                if (handler.IsValid())
                    Addressables.Release(handler);
                s_loadingAssets.Remove(runtimeKey);
                return null;
            }
        }

        public static TType GetOrLoadObject<TType>(this AssetReference assetRef)
            where TType : Object
        {
            object runtimeKey = assetRef.RuntimeKey;
            if (s_loadedAssets.TryGetValue(runtimeKey, out AsyncOperationHandle loadedHandle))
                return loadedHandle.Result as TType;

            // Check if the asset is actually marked as Addressable
            if (!assetRef.IsDataValid())
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Asset is not marked as Addressable: {runtimeKey}. Ignoring load.");
#endif
                s_loadingAssets.Remove(runtimeKey);
                return null;
            }

            // Check if the Addressable asset exists before loading
            AsyncOperationHandle<IList<IResourceLocation>> loadResourceLocationsHandle = Addressables.LoadResourceLocationsAsync(runtimeKey);
            IList<IResourceLocation> locations = loadResourceLocationsHandle.WaitForCompletion();
            loadResourceLocationsHandle.Release();
            if (locations == null || locations.Count == 0)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning($"Addressable asset not found: {runtimeKey}. Ignoring load.");
#endif
                return null;
            }

            try
            {
                AsyncOperationHandle<TType> handler = Addressables.LoadAssetAsync<TType>(runtimeKey);
                TType result = handler.WaitForCompletion();
                s_loadedAssets[assetRef.RuntimeKey] = handler;
                s_loadingAssets.Remove(runtimeKey);
                return result;
            }
            catch (System.Exception ex)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError($"Failed to load addressable asset: {runtimeKey}, {ex.Message}\n{ex.StackTrace}");
#endif
                s_loadingAssets.Remove(runtimeKey);
                return null;
            }
        }

        public static async UniTask<TType> GetOrLoadAssetAsync<TType>(this AssetReference assetRef)
            where TType : Component
        {
            GameObject loadedObject = await assetRef.GetOrLoadAssetAsync();
            if (loadedObject != null)
                return loadedObject.GetComponent<TType>();
            return null;
        }

        public static TType GetOrLoadAsset<TType>(this AssetReference assetRef)
            where TType : Component
        {
            GameObject loadedObject = assetRef.GetOrLoadAsset();
            if (loadedObject != null)
                return loadedObject.GetComponent<TType>();
            return null;
        }

        public static async UniTask<GameObject> GetOrLoadAssetAsync(this AssetReference assetRef)
        {
            return await assetRef.GetOrLoadObjectAsync<GameObject>();
        }

        public static GameObject GetOrLoadAsset(this AssetReference assetRef)
        {
            return assetRef.GetOrLoadObject<GameObject>();
        }

        public static async UniTask<TType> GetOrLoadAssetAsyncOrUsePrefab<TType>(this AssetReference assetRef, TType prefab)
            where TType : Component
        {
            TType tempPrefab = null;
            if (assetRef.IsDataValid())
                tempPrefab = await assetRef.GetOrLoadAssetAsync<TType>();
            if (tempPrefab == null)
                tempPrefab = prefab;
            return tempPrefab;
        }

        public static TType GetOrLoadAssetOrUsePrefab<TType>(this AssetReference assetRef, TType prefab)
            where TType : Component
        {
            TType tempPrefab = null;
            if (assetRef.IsDataValid())
                tempPrefab = assetRef.GetOrLoadAsset<TType>();
            if (tempPrefab == null)
                tempPrefab = prefab;
            return tempPrefab;
        }

        public static async UniTask<TType> GetOrLoadObjectAsyncOrUseAsset<TType>(this AssetReference assetRef, TType asset)
            where TType : Object
        {
            TType tempAsset = null;
            if (assetRef.IsDataValid())
                tempAsset = await assetRef.GetOrLoadObjectAsync<TType>();
            if (tempAsset == null)
                tempAsset = asset;
            return tempAsset;
        }

        public static TType GetOrLoadObjectOrUseAsset<TType>(this AssetReference assetRef, TType asset)
            where TType : Object
        {
            TType tempAsset = null;
            if (assetRef.IsDataValid())
                tempAsset = assetRef.GetOrLoadObject<TType>();
            if (tempAsset == null)
                tempAsset = asset;
            return tempAsset;
        }

        public static async UniTask<GameObject> GetOrLoadAssetAsyncOrUsePrefab(this AssetReference assetRef, GameObject prefab)
        {
            GameObject tempPrefab = null;
            if (assetRef.IsDataValid())
                tempPrefab = await assetRef.GetOrLoadAssetAsync();
            if (tempPrefab == null)
                tempPrefab = prefab;
            return tempPrefab;
        }

        public static GameObject GetOrLoadAssetOrUsePrefab(this AssetReference assetRef, GameObject prefab)
        {
            GameObject tempPrefab = null;
            if (assetRef.IsDataValid())
                tempPrefab = assetRef.GetOrLoadAsset();
            if (tempPrefab == null)
                tempPrefab = prefab;
            return tempPrefab;
        }

        public static async UniTask<TType[]> GetOrLoadObjectsAsync<TType>(this IEnumerable<AssetReference> assetRefs)
            where TType : Object
        {
            List<UniTask<TType>> tasks = new List<UniTask<TType>>();
            foreach (AssetReference assetRef in assetRefs)
            {
                tasks.Add(assetRef.GetOrLoadObjectAsync<TType>());
            }
            return await UniTask.WhenAll(tasks);
        }

        public static TType[] GetOrLoadObjects<TType>(this IEnumerable<AssetReference> assetRefs)
            where TType : Object
        {
            List<TType> results = new List<TType>();
            foreach (AssetReference assetRef in assetRefs)
            {
                results.Add(assetRef.GetOrLoadObject<TType>());
            }
            return results.ToArray();
        }

        public static async UniTask<TType[]> GetOrLoadAssetsAsync<TType>(this IEnumerable<AssetReference> assetRefs)
            where TType : Component
        {
            List<UniTask<TType>> tasks = new List<UniTask<TType>>();
            foreach (AssetReference assetRef in assetRefs)
            {
                tasks.Add(assetRef.GetOrLoadAssetAsync<TType>());
            }
            return await UniTask.WhenAll(tasks);
        }

        public static TType[] GetOrLoadAssets<TType>(this IEnumerable<AssetReference> assetRefs)
            where TType : Component
        {
            List<TType> results = new List<TType>();
            foreach (AssetReference assetRef in assetRefs)
            {
                results.Add(assetRef.GetOrLoadAsset<TType>());
            }
            return results.ToArray();
        }

        public static async UniTask<GameObject[]> GetOrLoadAssetsAsync(this IEnumerable<AssetReference> assetRefs)
        {
            List<UniTask<GameObject>> tasks = new List<UniTask<GameObject>>();
            foreach (AssetReference assetRef in assetRefs)
            {
                tasks.Add(assetRef.GetOrLoadAssetAsync());
            }
            return await UniTask.WhenAll(tasks);
        }

        public static GameObject[] GetOrLoadAssets(this IEnumerable<AssetReference> assetRefs)
        {
            List<GameObject> results = new List<GameObject>();
            foreach (AssetReference assetRef in assetRefs)
            {
                results.Add(assetRef.GetOrLoadAsset());
            }
            return results.ToArray();
        }

        public static async UniTask InstantiateObjectsOrUsePrefabs(this AssetReference[] addressablePrefabs, GameObject[] prefabs, Transform transform, List<GameObject> instantiatedObjects = null)
        {
            if ((prefabs == null || prefabs.Length <= 0) && addressablePrefabs != null && addressablePrefabs.Length > 0)
            {
                List<UniTask<GameObject>> instantiateTasks = new List<UniTask<GameObject>>();
                foreach (AssetReference addressablePrefab in addressablePrefabs)
                {
                    if (!addressablePrefab.IsDataValid())
                        continue;
                    instantiateTasks.Add(Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, transform.position, transform.rotation, transform, true).ToUniTask());
                }
                await UniTask.WhenAll(instantiateTasks);
                if (instantiatedObjects != null)
                {
                    foreach (UniTask<GameObject> task in instantiateTasks)
                    {
                        if (task.Status != UniTaskStatus.Succeeded)
                            continue;
                        instantiatedObjects.Add(task.GetAwaiter().GetResult());
                    }
                }
                return;
            }
            if (prefabs != null && prefabs.Length > 0)
            {
                foreach (GameObject prefab in prefabs)
                {
                    if (prefab == null) continue;
                    GameObject instantiatedObject = Object.Instantiate(prefab, transform.position, transform.rotation, transform);
                    if (instantiatedObjects != null && instantiatedObject != null)
                        instantiatedObjects.Add(instantiatedObject);
                }
            }
        }

        public static async UniTask InstantiateObjectsOrUsePrefabs(this AssetReference[] addressablePrefabs, GameObject[] prefabs, Vector3 position, Quaternion rotation, Transform transform = null, List<GameObject> instantiatedObjects = null)
        {
            if ((prefabs == null || prefabs.Length <= 0) && addressablePrefabs != null && addressablePrefabs.Length > 0)
            {
                List<UniTask<GameObject>> instantiateTasks = new List<UniTask<GameObject>>();
                foreach (AssetReference addressablePrefab in addressablePrefabs)
                {
                    if (!addressablePrefab.IsDataValid())
                        continue;
                    instantiateTasks.Add(Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, position, rotation, transform, true).ToUniTask());
                }
                await UniTask.WhenAll(instantiateTasks);
                if (instantiatedObjects != null)
                {
                    foreach (UniTask<GameObject> task in instantiateTasks)
                    {
                        if (task.Status != UniTaskStatus.Succeeded)
                            continue;
                        instantiatedObjects.Add(task.GetAwaiter().GetResult());
                    }
                }
                return;
            }
            if (prefabs != null && prefabs.Length > 0)
            {
                foreach (GameObject prefab in prefabs)
                {
                    if (prefab == null) continue;
                    GameObject instantiatedObject;
                    if (transform == null)
                        instantiatedObject = Object.Instantiate(prefab, position, rotation);
                    else
                        instantiatedObject = Object.Instantiate(prefab, position, rotation, transform);
                    if (instantiatedObjects != null && instantiatedObject != null)
                        instantiatedObjects.Add(instantiatedObject);
                }
            }
        }

        public static async UniTask<GameObject> InstantiateObjectOrUsePrefab(this AssetReference addressablePrefab, GameObject prefab, Transform transform)
        {
            if (prefab == null && addressablePrefab.IsDataValid())
            {
                return await Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, transform.position, transform.rotation, transform, true).ToUniTask();
            }
            if (prefab != null)
            {
                return Object.Instantiate(prefab, transform.position, transform.rotation, transform);
            }
            return null;
        }

        public static async UniTask<T> InstantiateObjectOrUsePrefab<T>(this AssetReference addressablePrefab, GameObject prefab, Transform transform)
            where T : Component
        {
            GameObject instantiatedObject = await addressablePrefab.InstantiateObjectOrUsePrefab(prefab, transform);
            if (instantiatedObject != null)
                return instantiatedObject.GetComponent<T>();
            return null;
        }

        public static async UniTask<GameObject> InstantiateObjectOrUsePrefab(this AssetReference addressablePrefab, GameObject prefab, Vector3 position, Quaternion rotation, Transform transform = null)
        {
            if (prefab == null && addressablePrefab.IsDataValid())
            {
                return await Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, position, rotation, transform, true).ToUniTask();
            }
            if (prefab != null)
            {
                if (transform == null)
                    return Object.Instantiate(prefab, position, rotation);
                else
                    return Object.Instantiate(prefab, position, rotation, transform);
            }
            return null;
        }

        public static async UniTask<T> InstantiateObjectOrUsePrefab<T>(this AssetReference addressablePrefab, GameObject prefab, Vector3 position, Quaternion rotation, Transform transform = null)
            where T : Component
        {
            GameObject instantiatedObject = await addressablePrefab.InstantiateObjectOrUsePrefab(prefab, position, rotation, transform);
            if (instantiatedObject != null)
                return instantiatedObject.GetComponent<T>();
            return null;
        }

        public static async UniTask<GameObject> InstantiateObject(this AssetReference addressablePrefab, Transform transform)
        {
            if (addressablePrefab.IsDataValid())
            {
                return await Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, transform.position, transform.rotation, transform, true).ToUniTask();
            }
            return null;
        }

        public static async UniTask<T> InstantiateObject<T>(this AssetReference addressablePrefab, Transform transform)
            where T : Component
        {
            GameObject instantiatedObject = await addressablePrefab.InstantiateObject(transform);
            if (instantiatedObject != null)
                return instantiatedObject.GetComponent<T>();
            return null;
        }

        public static async UniTask<GameObject> InstantiateObject(this AssetReference addressablePrefab, Vector3 position, Quaternion rotation, Transform transform = null)
        {
            if (addressablePrefab.IsDataValid())
            {
                return await Addressables.InstantiateAsync(addressablePrefab.RuntimeKey, position, rotation, transform, true).ToUniTask();
            }
            return null;
        }

        public static async UniTask<T> InstantiateObject<T>(this AssetReference addressablePrefab, Vector3 position, Quaternion rotation, Transform transform = null)
            where T : Component
        {
            GameObject instantiatedObject = await addressablePrefab.InstantiateObject(position, rotation, transform);
            if (instantiatedObject != null)
                return instantiatedObject.GetComponent<T>();
            return null;
        }

        public static void Release(object runtimeKey)
        {
            if (s_loadedAssets.TryGetValue(runtimeKey, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
            }
            s_loadedAssets.Remove(runtimeKey);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log($"Released addressable asset: {runtimeKey}");
#endif
        }

        public static void ReleaseAll()
        {
            List<object> keys = new List<object>(s_loadedAssets.Keys);
            for (int i = 0; i < keys.Count; ++i)
            {
                Release(keys[i]);
            }
        }
    }
}
#endif