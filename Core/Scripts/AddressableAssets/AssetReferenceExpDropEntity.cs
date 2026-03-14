#if !DISABLE_ADDRESSABLES
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceExpDropEntity : AssetReferenceBaseRewardDropEntity
    {
        public AssetReferenceExpDropEntity(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset<ExpDropEntity>(obj);
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset<ExpDropEntity>(path);
        }
    }
}
#endif