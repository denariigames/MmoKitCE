#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBasePlayerCharacterController : AssetReferenceComponent<BasePlayerCharacterController>
    {
        public AssetReferenceBasePlayerCharacterController(string guid) : base(guid)
        {
        }
    }
}
#endif