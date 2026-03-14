using UnityEngine.Events;

namespace Insthync.AddressableAssetTools
{
    /// <summary>
    /// Loaded Count, Total Count
    /// </summary>
    [System.Serializable]
    public class AddressableAssetTotalProgressEvent : UnityEvent<int, int>
    {
    }
}
