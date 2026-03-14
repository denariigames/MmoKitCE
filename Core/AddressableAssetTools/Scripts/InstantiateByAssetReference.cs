#if !DISABLE_ADDRESSABLES
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Insthync.AddressableAssetTools
{
    public class InstantiateByAssetReference : MonoBehaviour
    {
        public AssetReference assetReference;
        public bool instantiateOnStart = false;
        [Tooltip("If `destroyDelay` is greater than 0, the instantiated object will be destroyed after the delay.")]
        public float destroyDelay = 0f;
        public Transform parent;

        void Start()
        {
            if (instantiateOnStart)
            {
                if (parent == null)
                    parent = transform;
                Instantiate(parent);
            }
        }

        public virtual async UniTask<GameObject> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!assetReference.IsDataValid())
                Debug.LogWarning("AssetReference is not valid.");
            return await Addressables.InstantiateAsync(assetReference.RuntimeKey, position, rotation, parent, true).ToUniTask();
        }

        public virtual async UniTask<GameObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            if (!assetReference.IsDataValid())
                Debug.LogWarning("AssetReference is not valid.");
            return await Addressables.InstantiateAsync(assetReference.RuntimeKey, parent, instantiateInWorldSpace, true).ToUniTask();
        }
    }
}
#endif