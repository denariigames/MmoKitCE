using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class GameEffectPoolContainer : IAddressableAssetConversable
    {
#if !UNITY_SERVER || UNITY_EDITOR
        public Transform container;
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public GameEffect prefab;
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect addressablePrefab;
#endif
#endif

        public async void GetInstance()
        {
#if !UNITY_SERVER
            GameEffect tempPrefab = null;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            tempPrefab = prefab;
#endif
#if !DISABLE_ADDRESSABLES
            AssetReferenceGameEffect tempAddressablePrefab = addressablePrefab;
#endif
            GameEffect loadedPrefab;
#if !DISABLE_ADDRESSABLES
            loadedPrefab = await tempAddressablePrefab.GetOrLoadAssetAsyncOrUsePrefab(tempPrefab);
#else
            await UniTask.Yield();
            loadedPrefab = tempPrefab;
#endif
            if (loadedPrefab != null)
                PoolSystem.GetInstance(loadedPrefab, container.position, container.rotation).FollowingTarget = container;
#endif
        }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref prefab, ref addressablePrefab, groupName);
#endif
        }
    }
}