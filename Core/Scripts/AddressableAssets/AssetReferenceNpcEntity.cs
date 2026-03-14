#if !DISABLE_ADDRESSABLES
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceNpcEntity : AssetReferenceLiteNetLibBehaviour<NpcEntity>
    {
        public AssetReferenceNpcEntity(string guid) : base(guid)
        {
        }
    }
}
#endif