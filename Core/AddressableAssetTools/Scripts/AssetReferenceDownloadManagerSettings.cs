#if !DISABLE_ADDRESSABLES
namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AssetReferenceDownloadManagerSettings : AssetReferenceScriptableObject<AddressableAssetDownloadManagerSettings>
    {
        public AssetReferenceDownloadManagerSettings(string guid) : base(guid)
        {
        }
    }
}
#endif