#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Insthync.AddressableAssetTools
{
    public class AssetReferenceReleaser : MonoBehaviour
    {
        private void OnDestroy()
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}
#endif