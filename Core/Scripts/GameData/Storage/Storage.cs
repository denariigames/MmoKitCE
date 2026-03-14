using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct Storage
    {
        [Tooltip("If weight limit <= 0, assume that it has no limit")]
        public int weightLimit;
        [Tooltip("If slot limit <= 0, assume that it has no limit")]
        public int slotLimit;
    }
}
