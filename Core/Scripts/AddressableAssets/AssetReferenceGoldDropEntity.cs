#if !DISABLE_ADDRESSABLES
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceGoldDropEntity : AssetReferenceBaseRewardDropEntity
    {
        public AssetReferenceGoldDropEntity(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset<GoldDropEntity>(obj);
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset<GoldDropEntity>(path);
        }
    }
}
#endif