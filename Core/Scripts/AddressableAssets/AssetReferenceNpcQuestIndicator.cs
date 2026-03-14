#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceNpcQuestIndicator : AssetReferenceComponent<NpcQuestIndicator>
    {
        public AssetReferenceNpcQuestIndicator(string guid) : base(guid)
        {
        }
    }
}
#endif