using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct IgnoreColliderInfo
    {
        public bool defaultIgnoreStatus;
        public Collider colliderA;
        public Collider colliderB;
    }
}
