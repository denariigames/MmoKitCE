#if !DISABLE_ADDRESSABLES
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceCurrencyDropEntity : AssetReferenceBaseRewardDropEntity
    {
        public AssetReferenceCurrencyDropEntity(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset<CurrencyDropEntity>(obj);
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset<CurrencyDropEntity>(path);
        }
    }
}
#endif