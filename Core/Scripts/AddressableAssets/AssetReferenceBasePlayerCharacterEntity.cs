#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBasePlayerCharacterEntity : AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>
    {
        public AssetReferenceBasePlayerCharacterEntity(string guid) : base(guid)
        {
        }
    }
}
#endif