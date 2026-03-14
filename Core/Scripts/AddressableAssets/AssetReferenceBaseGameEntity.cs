#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBaseGameEntity : AssetReferenceLiteNetLibBehaviour<BaseGameEntity>
    {
        public AssetReferenceBaseGameEntity(string guid) : base(guid)
        {
        }
    }
}
#endif