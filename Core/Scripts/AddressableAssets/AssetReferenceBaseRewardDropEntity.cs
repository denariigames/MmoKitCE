#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBaseRewardDropEntity : AssetReferenceLiteNetLibBehaviour<BaseRewardDropEntity>
    {
        public AssetReferenceBaseRewardDropEntity(string guid) : base(guid)
        {
        }
    }
}
#endif