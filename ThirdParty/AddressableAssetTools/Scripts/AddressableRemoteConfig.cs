using System.Collections.Generic;

namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AddressableRemoteConfig
    {
        public Dictionary<string, string> replaceRuntimeProperties = new Dictionary<string, string>();
        public List<string> catalogUrls = new List<string>();
    }
}
