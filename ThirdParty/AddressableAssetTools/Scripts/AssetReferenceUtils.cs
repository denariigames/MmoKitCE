#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Insthync.AddressableAssetTools
{
    public static class AssetReferenceUtils
    {
        public static bool IsDataValid(this AssetReference asset)
        {
            return asset != null && asset.RuntimeKeyIsValid();
        }

        public static AsyncOperationHandle<T> CreateGetComponentCompletedOperation<T>(AsyncOperationHandle<GameObject> handler)
        {
            return Addressables.ResourceManager.CreateCompletedOperation(handler.Result.GetComponent<T>(), string.Empty);
        }
    }
}
#endif