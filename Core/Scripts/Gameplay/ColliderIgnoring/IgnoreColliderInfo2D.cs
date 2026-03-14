using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct IgnoreColliderInfo2D
    {
        public bool defaultIgnoreStatus;
        public Collider2D colliderA;
        public Collider2D colliderB;
    }
}
