using UnityEngine;

namespace MultiplayerARPG
{
    public struct PhysicRaycastResult
    {
        /// <summary>
        /// The impact point in world space where the ray hit the collider.
        /// </summary>
        public Vector3 point { get; set; }

        /// <summary>
        /// The normal of the surface the ray hit.
        /// </summary>
        public Vector3 normal { get; set; }

        /// <summary>
        /// The distance from the ray's origin to the impact point.
        /// </summary>
        public float distance { get; set; }

        /// <summary>
        /// The Transform of the rigidbody or collider that was hit.
        /// </summary>
        public Transform transform { get; set; }
    }
}
