using UnityEngine;

namespace MultiplayerARPG
{
    public class AOIMapBounds : MonoBehaviour
    {
        public enum EMode
        {
            Bounds,
            TransformAndSize,
        }
        public EMode mode = EMode.Bounds;
        public Bounds bounds = default;
        public Vector3 size = default;

        public Bounds GetBounds()
        {
            switch (mode)
            {
                case EMode.TransformAndSize:
                    return new Bounds(transform.position, size);
                default:
                    return bounds;
            }
        }

#if UNITY_EDITOR
        public Color gizmosColor = Color.cyan;
        private void OnDrawGizmos()
        {
            Color prevColor = Gizmos.color;
            Gizmos.color = gizmosColor;
            Bounds bounds = GetBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color = prevColor;
        }
#endif
    }
}
