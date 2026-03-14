using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ConvertItem
    {
        [Tooltip("Item which is source to convert, it also can be a fuel")]
        public ItemAmount item;
        [Tooltip("If this is `TRUE` this item will be required to use as a fuel")]
        public bool isFuel;
        [Tooltip("Inverval to decrease amount of `item`")]
        public float convertInterval;
        [Tooltip("Result after the `item` converted (or burnt if it's a fuel)")]
        public ItemAmount convertedItem;
    }
}
