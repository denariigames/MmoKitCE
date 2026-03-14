using UnityEngine;

namespace MultiplayerARPG
{
    public class DropEntityUtils
    {
        public const float ALLOWED_HEIGHT_DIST = 0.75f;
        public const float LOWEST_DROP_DISTANCE = 0.125f;
        public const float GROUND_DETECTION_DISTANCE = 100f;
        public const byte MAX_FIND_GROUND_RETIRES = 8;
        private static readonly RaycastHit[] s_findGroundRaycastHits = new RaycastHit[8];

        public static Vector3 GetDroppedPosition3D(Vector3 origin, float dropDistance)
        {
            Vector3 randomPos;
            Vector3 groundedPos;
            for (int i = 0; i < MAX_FIND_GROUND_RETIRES; ++i)
            {
                randomPos = origin + new Vector3(Random.Range(-1f, 1f) * dropDistance, 0f, Random.Range(-1f, 1f) * dropDistance);
                groundedPos = PhysicUtils.FindGroundedPosition(randomPos, s_findGroundRaycastHits, GROUND_DETECTION_DISTANCE, GameInstance.Singleton.GetItemDropGroundDetectionLayerMask());
                if (Mathf.Abs(groundedPos.y - origin.y) < ALLOWED_HEIGHT_DIST)
                    return groundedPos;
            }
            randomPos = origin + new Vector3(Random.Range(-1f, 1f) * LOWEST_DROP_DISTANCE, 0f, Random.Range(-1f, 1f) * LOWEST_DROP_DISTANCE);
            groundedPos = PhysicUtils.FindGroundedPosition(randomPos, s_findGroundRaycastHits, GROUND_DETECTION_DISTANCE, GameInstance.Singleton.GetItemDropGroundDetectionLayerMask());
            if (Mathf.Abs(groundedPos.y - origin.y) < ALLOWED_HEIGHT_DIST)
                return groundedPos;
            // May float
            return randomPos;
        }

        public static Vector3 GetDroppedPosition2D(Vector3 origin, float dropDistance)
        {
            return origin + new Vector3(Random.Range(-1f, 1f) * dropDistance, Random.Range(-1f, 1f) * dropDistance);
        }
    }
}
