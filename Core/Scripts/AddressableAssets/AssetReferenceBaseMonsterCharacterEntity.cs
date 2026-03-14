#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceBaseMonsterCharacterEntity : AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>
    {
        public AssetReferenceBaseMonsterCharacterEntity(string guid) : base(guid)
        {
        }
    }
}
#endif