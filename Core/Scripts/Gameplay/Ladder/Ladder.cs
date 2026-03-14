using UnityEngine;

namespace MultiplayerARPG
{
    public class Ladder : MonoBehaviour
    {
        public Transform bottomTransform;
        public Transform topTransform;
        public Transform bottomExitTransform;
        public Transform topExitTransform;
        public float yAngleOffsets = 0f;
        public Vector3 Up => (topTransform.position - bottomTransform.position).normalized;
        public Vector3 Right => Vector3.Cross(Up, Vector3.forward).normalized;
        public Vector3 Forward => Vector3.Cross(Right, Up).normalized;
        public Vector3 RightWithYAngleOffsets => Quaternion.Euler(0, yAngleOffsets, 0) * Right;
        public Vector3 ForwardWithYAngleOffsets => Quaternion.Euler(0, yAngleOffsets, 0) * Forward;

        public Vector3 ClosestPointOnLadderSegment(Vector3 fromPoint, float forwardOffsets, out float onSegmentState)
        {
            Vector3 segment = topTransform.position - bottomTransform.position;
            Vector3 segmentPoint1ToPoint = fromPoint - bottomTransform.position;
            float pointProjectionLength = Vector3.Dot(segmentPoint1ToPoint, segment.normalized);

            // When higher than bottom point
            if (pointProjectionLength > 0)
            {
                // If we are not higher than top point
                if (pointProjectionLength <= segment.magnitude)
                {
                    onSegmentState = 0;
                    return bottomTransform.position + (segment.normalized * pointProjectionLength) + (ForwardWithYAngleOffsets * forwardOffsets);
                }
                // If we are higher than top point
                else
                {
                    onSegmentState = pointProjectionLength - segment.magnitude;
                    return topTransform.position + (ForwardWithYAngleOffsets * forwardOffsets);
                }
            }
            // When lower than bottom point
            else
            {
                onSegmentState = pointProjectionLength;
                return bottomTransform.position + (ForwardWithYAngleOffsets * forwardOffsets);
            }
        }

        private void OnDrawGizmos()
        {
            if (bottomTransform == null || topTransform == null)
                return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bottomTransform.position, topTransform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(bottomTransform.position - transform.right * 0.1f, bottomTransform.position + transform.right * 0.1f);
            Gizmos.DrawLine(bottomTransform.position, bottomTransform.position + ForwardWithYAngleOffsets * 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(topTransform.position - transform.right * 0.1f, topTransform.position + transform.right * 0.1f);
            Gizmos.DrawLine(topTransform.position, topTransform.position + ForwardWithYAngleOffsets * 0.1f);
            if (bottomExitTransform == null || topExitTransform == null)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(bottomExitTransform.position, Vector3.one * 0.25f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(topExitTransform.position, Vector3.one * 0.25f);
        }
    }
}
