using UnityEngine.Events;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class DiscoveryDataEvent : UnityEvent<DiscoveryData> { }

    [System.Serializable]
    public class UIDiscoveryEntryEvent : UnityEvent<UIDiscoveryEntry> { }

    public class UIDiscoveryEntrySelectionManager : UISelectionManager<DiscoveryData, UIDiscoveryEntry, DiscoveryDataEvent, UIDiscoveryEntryEvent>
    {
    }
}
