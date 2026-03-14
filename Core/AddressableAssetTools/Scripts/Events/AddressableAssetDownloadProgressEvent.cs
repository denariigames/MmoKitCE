using UnityEngine.Events;

namespace Insthync.AddressableAssetTools
{
    /// <summary>
    /// Args - downloadedSize: long, fileSize: long, percentComplete: float (1 = 100%)
    /// </summary>
    [System.Serializable]
    public class AddressableAssetDownloadProgressEvent : UnityEvent<long, long, float>
    {
    }
}
