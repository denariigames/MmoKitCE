using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class EntityMovementFunctions
    {
        internal const int FIND_GROUND_HIT_ARRAY_LENGTH = 16;
        internal static readonly Vector3[] s_changePoseRaycastOffsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(0f, 0f, 1f),
            new Vector3(0f, 0f, -1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0.75f, 0f, 0.75f),
            new Vector3(0.75f, 0f, -0.75f),
            new Vector3(-0.75f, 0f, 0.75f),
            new Vector3(-0.75f, 0f, -0.75f),
        };
        internal static readonly List<Vector3> s_findGroundHitPoints = new List<Vector3>();

        #region Generic Functions
        public static ExtraMovementState ValidateExtraMovementState(this IEntityMovement movement, MovementState movementState, ExtraMovementState extraMovementState)
        {
            // Movement state can affect extra movement state
            if (movementState.Has(MovementState.IsUnderWater) ||
                movementState.Has(MovementState.IsClimbing))
            {
                // Extra movement states always none while under water
                extraMovementState = ExtraMovementState.None;
            }
            else if (!movement.Entity.CanMove() && extraMovementState == ExtraMovementState.IsSprinting)
            {
                // Character can't move, set extra movement state to none
                extraMovementState = ExtraMovementState.None;
            }
            else
            {
                switch (extraMovementState)
                {
                    case ExtraMovementState.IsSprinting:
                        if (!movementState.HasDirectionMovement())
                            extraMovementState = ExtraMovementState.None;
                        else if (!movement.Entity.CanSprint())
                            extraMovementState = ExtraMovementState.None;
                        else if (!movement.Entity.CanSideSprint && (movementState.Has(MovementState.Left) || movementState.Has(MovementState.Right)))
                            extraMovementState = ExtraMovementState.None;
                        else if (!movement.Entity.CanBackwardSprint && movementState.Has(MovementState.Backward))
                            extraMovementState = ExtraMovementState.None;
                        break;
                    case ExtraMovementState.IsWalking:
                        if (!movementState.HasDirectionMovement())
                            extraMovementState = ExtraMovementState.None;
                        else if (!movement.Entity.CanWalk())
                            extraMovementState = ExtraMovementState.None;
                        break;
                    case ExtraMovementState.IsCrouching:
                        if (!movement.Entity.CanCrouch())
                            extraMovementState = ExtraMovementState.None;
                        break;
                    case ExtraMovementState.IsCrawling:
                        if (!movement.Entity.CanCrawl())
                            extraMovementState = ExtraMovementState.None;
                        break;
                }
            }
            return extraMovementState;
        }
        #endregion

        #region Movement Input Serialization (3D)
        public static void ClientWriteMovementInput3D(this IEntityMovement movement, NetDataWriter writer, EntityMovementInputState inputState, EntityMovementInput movementInput)
        {
            if (!movement.Entity.IsOwnerClient)
                return;
            writer.Put((byte)inputState);
            writer.PutPackedUInt((uint)movementInput.MovementState);
            if (!inputState.Has(EntityMovementInputState.IsStopped))
                writer.Put((byte)movementInput.ExtraMovementState);
            if (inputState.Has(EntityMovementInputState.PositionChanged))
                writer.PutVector3(movementInput.Position);
            if (inputState.Has(EntityMovementInputState.RotationChanged))
                writer.PutPackedInt(GetCompressedAngle(movementInput.YAngle));
        }

        public static void ReadMovementInputMessage3D(this NetDataReader reader, out EntityMovementInputState inputState, out EntityMovementInput entityMovementInput)
        {
            entityMovementInput = new EntityMovementInput();
            inputState = (EntityMovementInputState)reader.GetByte();
            entityMovementInput.MovementState = (MovementState)reader.GetPackedUInt();
            if (!inputState.Has(EntityMovementInputState.IsStopped))
                entityMovementInput.ExtraMovementState = (ExtraMovementState)reader.GetByte();
            else
                entityMovementInput.ExtraMovementState = ExtraMovementState.None;
            if (inputState.Has(EntityMovementInputState.PositionChanged))
                entityMovementInput.Position = reader.GetVector3();
            if (inputState.Has(EntityMovementInputState.RotationChanged))
                entityMovementInput.YAngle = GetDecompressedAngle(reader.GetPackedInt());
        }
        #endregion

        #region Movement Input Serialization (2D)
        public static void ClientWriteMovementInput2D(this IEntityMovement movement, NetDataWriter writer, EntityMovementInputState inputState, EntityMovementInput movementInput)
        {
            if (!movement.Entity.IsOwnerClient)
                return;
            writer.Put((byte)inputState);
            writer.PutPackedUInt((uint)movementInput.MovementState);
            if (!inputState.Has(EntityMovementInputState.IsStopped))
                writer.Put((byte)movementInput.ExtraMovementState);
            if (inputState.Has(EntityMovementInputState.PositionChanged))
                writer.PutVector2(movementInput.Position);
            writer.Put(movementInput.Direction2D);
        }

        public static void ReadMovementInputMessage2D(this NetDataReader reader, out EntityMovementInputState inputState, out EntityMovementInput entityMovementInput)
        {
            entityMovementInput = new EntityMovementInput();
            inputState = (EntityMovementInputState)reader.GetByte();
            entityMovementInput.MovementState = (MovementState)reader.GetPackedUInt();
            if (!inputState.Has(EntityMovementInputState.IsStopped))
                entityMovementInput.ExtraMovementState = (ExtraMovementState)reader.GetByte();
            else
                entityMovementInput.ExtraMovementState = ExtraMovementState.None;
            if (inputState.Has(EntityMovementInputState.PositionChanged))
                entityMovementInput.Position = reader.GetVector2();
            entityMovementInput.Direction2D = reader.Get<DirectionVector2>();
        }
        #endregion

        #region Sync Transform Serialization (3D)
        public static void ServerWriteSyncTransform3D(this IEntityMovement movement, List<EntityMovementForceApplier> movementForceAppliers, int _lastDataCompressionMode, NetDataWriter writer)
        {
            if (!movement.Entity.IsServer)
                return;
            writer.PutPackedUInt((uint)movement.MovementState);
            writer.Put((byte)movement.ExtraMovementState);
            //get cell id and local position to reduce the amount of data to sync
            if (!DefaultGridManagerComponent.Instance.IsDisabled)
            {
                GetCellIdAndLocalPosition(movement.Entity.EntityTransform.position, out byte cellId, out Vector3 localposition);
                writer.Put(cellId);
                writer.PutQuantizedVector3(localposition, DefaultGridManagerComponent.Instance.CellSize, _lastDataCompressionMode);
            }
            else
                writer.PutVector3(movement.Entity.EntityTransform.position);
            writer.Put(CompressAngle(movement.Entity.EntityTransform.eulerAngles.y));
            writer.PutList(movementForceAppliers);
        }

        public static void ClientWriteSyncTransform3D(this IEntityMovement movement, NetDataWriter writer)
        {
            if (!movement.Entity.IsOwnerClient)
                return;
            writer.PutPackedUInt((uint)movement.MovementState);
            writer.Put((byte)movement.ExtraMovementState);
            writer.PutVector3(movement.Entity.EntityTransform.position);
            writer.PutPackedInt(GetCompressedAngle(movement.Entity.EntityTransform.eulerAngles.y));
        }

        public static void ServerReadSyncTransformMessage3D(this NetDataReader reader, out MovementState movementState, out ExtraMovementState extraMovementState, out Vector3 position, out float yAngle)
        {
            movementState = (MovementState)reader.GetPackedUInt();
            extraMovementState = (ExtraMovementState)reader.GetByte();
            position = reader.GetVector3();
            yAngle = GetDecompressedAngle(reader.GetPackedInt());
        }

        public static void ClientReadSyncTransformMessage3D(this NetDataReader reader, out MovementState movementState, out ExtraMovementState extraMovementState, out Vector3 position, out float yAngle, out List<EntityMovementForceApplier> movementForceAppliers)
        {
            movementState = (MovementState)reader.GetPackedUInt();
            extraMovementState = (ExtraMovementState)reader.GetByte();
            if (!DefaultGridManagerComponent.Instance.IsDisabled)
            {
                byte cellId = reader.GetByte();
                Vector3 localPosition = reader.GetQuantizedVector3(DefaultGridManagerComponent.Instance.CellSize, out int compressionMode);
                position = DefaultGridManagerComponent.Instance.GetWorldPosition(cellId, localPosition);
            }
            else
                position = reader.GetVector3();

            yAngle = DecompressAngle(reader.GetByte());
            movementForceAppliers = reader.GetList<EntityMovementForceApplier>();
        }
        #endregion

        #region Sync Transform Serialization (2D)
        public static void ServerWriteSyncTransform2D(this IEntityMovement movement, List<EntityMovementForceApplier> movementForceAppliers, NetDataWriter writer)
        {
            if (!movement.Entity.IsServer)
                return;
            writer.PutPackedUInt((uint)movement.MovementState);
            writer.Put((byte)movement.ExtraMovementState);
            writer.PutVector2(movement.Entity.EntityTransform.position);
            writer.Put(movement.Direction2D);
            writer.PutList(movementForceAppliers);
        }

        public static void ClientWriteSyncTransform2D(this IEntityMovement movement, NetDataWriter writer)
        {
            if (!movement.Entity.IsOwnerClient)
                return;
            writer.PutPackedUInt((uint)movement.MovementState);
            writer.Put((byte)movement.ExtraMovementState);
            writer.PutVector2(movement.Entity.EntityTransform.position);
            writer.Put(movement.Direction2D);
        }

        public static void ServerReadSyncTransformMessage2D(this NetDataReader reader, out MovementState movementState, out ExtraMovementState extraMovementState, out Vector2 position, out DirectionVector2 direction2D)
        {
            movementState = (MovementState)reader.GetPackedUInt();
            extraMovementState = (ExtraMovementState)reader.GetByte();
            position = reader.GetVector2();
            direction2D = reader.Get<DirectionVector2>();
        }

        public static void ClientReadSyncTransformMessage2D(this NetDataReader reader, out MovementState movementState, out ExtraMovementState extraMovementState, out Vector2 position, out DirectionVector2 direction2D, out List<EntityMovementForceApplier> movementForceAppliers)
        {
            movementState = (MovementState)reader.GetPackedUInt();
            extraMovementState = (ExtraMovementState)reader.GetByte();
            position = reader.GetVector2();
            direction2D = reader.Get<DirectionVector2>();
            movementForceAppliers = reader.GetList<EntityMovementForceApplier>();
        }
        #endregion

        #region Helpers

        public static byte CompressAngle(float angle)
        {
            // normalize to 0–360
            angle %= 360f;
            if (angle < 0) angle += 360f;

            return (byte)(angle * (255f / 360f));
        }

        public static float DecompressAngle(byte value)
        {
            return value * (360f / 255f);
        }
        public static int GetCompressedAngle(float angle)
        {
            return Mathf.RoundToInt(angle * 6);
        }

        public static float GetDecompressedAngle(int compressedAngle)
        {
            return (float)compressedAngle / 6;
        }

        public static bool AllowToChangePose(this IEntityMovement movement, float height, float radius, int layerMask)
        {
            Transform transform = movement.Entity.EntityTransform;
            for (int i = 0; i < s_changePoseRaycastOffsets.Length; ++i)
            {
                Vector3 origin = (s_changePoseRaycastOffsets[i] * radius * (transform.lossyScale.x + transform.lossyScale.z) * 0.5f) + transform.position;
                RaycastHit[] findGroundRaycastHits = ArrayPool<RaycastHit>.Shared.Rent(FIND_GROUND_HIT_ARRAY_LENGTH);
                int hitCount = Physics.RaycastNonAlloc(
                    origin, Vector3.up,
                    findGroundRaycastHits, height * transform.lossyScale.y,
                    layerMask, QueryTriggerInteraction.Ignore);
                ArrayPool<RaycastHit>.Shared.Return(findGroundRaycastHits);
#if UNITY_EDITOR
                Debug.DrawLine(origin, origin + Vector3.up * height * transform.lossyScale.y, Color.blue, 30f);
#endif
                if (hitCount > 0)
                    return false;
            }
            return true;
        }

        public static void CalculateCrawlRaycastDegrees(int crawlCheckRaycasts, ref float[] result)
        {
            if (crawlCheckRaycasts > 0)
            {
                float increaseRaycastDegree = 360f / crawlCheckRaycasts;
                for (int i = 0; i < crawlCheckRaycasts; ++i)
                {
                    result[i] = i * increaseRaycastDegree;
                }
            }
        }

        public static bool AllowToCrawl(this IEntityMovement movement, int crawlCheckRaycasts, float crawlCheckOffsets, float crawlCheckRadius)
        {
            if (crawlCheckRaycasts <= 0)
                return true;
            Transform transform = movement.Entity.EntityTransform;
            Vector3 center = new Vector3(transform.position.x, transform.position.y - crawlCheckOffsets, transform.position.z);
            float[] crawlRaycastDegrees = ArrayPool<float>.Shared.Rent(crawlCheckRaycasts);
            RaycastHit[] findGroundRaycastHits = ArrayPool<RaycastHit>.Shared.Rent(FIND_GROUND_HIT_ARRAY_LENGTH);
            CalculateCrawlRaycastDegrees(crawlCheckRaycasts, ref crawlRaycastDegrees);
            for (int i = 0; i < crawlCheckRaycasts; ++i)
            {
                int hitCount = Physics.RaycastNonAlloc(
                    center, Quaternion.Euler(0f, crawlRaycastDegrees[i], 0f) * transform.forward,
                    findGroundRaycastHits, crawlCheckRadius,
                    GameInstance.Singleton.GetGameEntityGroundDetectionLayerMask(), QueryTriggerInteraction.Ignore);
                if (hitCount > 0)
                {
                    ArrayPool<float>.Shared.Return(crawlRaycastDegrees);
                    ArrayPool<RaycastHit>.Shared.Return(findGroundRaycastHits);
                    return false;
                }
            }
            ArrayPool<float>.Shared.Return(crawlRaycastDegrees);
            ArrayPool<RaycastHit>.Shared.Return(findGroundRaycastHits);
            return true;
        }

        public static Vector3 AdjustCrawlMotion(this IEntityMovement movement, MovementState movementState, ExtraMovementState extraMovementState, Vector3 motion, int crawlCheckRaycasts, float crawlCheckOffsets, float crawlCheckRadius, int layerMask)
        {
            if (extraMovementState != ExtraMovementState.IsCrawling || crawlCheckRaycasts == 0 || !movement.Entity.IsClient)
                return motion;
            Transform transform = movement.Entity.EntityTransform;
            Vector3 center = new Vector3(transform.position.x, transform.position.y - crawlCheckOffsets, transform.position.z);
            Vector3 moveDirection = motion.GetXZ().normalized;
            float nearestHitDistance = float.MaxValue;
            float nearestRaycastAngle = 0f;
            RaycastHit? nearestHit = null;
            float[] crawlRaycastDegrees = ArrayPool<float>.Shared.Rent(crawlCheckRaycasts);
            RaycastHit[] findGroundRaycastHits = ArrayPool<RaycastHit>.Shared.Rent(FIND_GROUND_HIT_ARRAY_LENGTH);
            CalculateCrawlRaycastDegrees(crawlCheckRaycasts, ref crawlRaycastDegrees);
            for (int i = 0; i < crawlCheckRaycasts; ++i)
            {
                Vector3 raycastDirection = Quaternion.Euler(0f, crawlRaycastDegrees[i], 0f) * moveDirection;
                float raycastAngle = Vector3.Angle(moveDirection, raycastDirection);
                if (raycastAngle > 90f)
                    continue;
                int hitCount = Physics.RaycastNonAlloc(center, raycastDirection, findGroundRaycastHits, crawlCheckRadius, layerMask, QueryTriggerInteraction.Ignore);
                if (hitCount <= 0)
                    continue;
                for (int j = 0; j < hitCount; ++j)
                {
                    RaycastHit hit = findGroundRaycastHits[j];
                    if (hit.distance >= nearestHitDistance)
                        continue;
                    nearestRaycastAngle = raycastAngle;
                    nearestHitDistance = hit.distance;
                    nearestHit = hit;
                }
            }
            ArrayPool<float>.Shared.Return(crawlRaycastDegrees);
            ArrayPool<RaycastHit>.Shared.Return(findGroundRaycastHits);
            if (nearestHit.HasValue)
            {
                Vector3 hitNormal = nearestHit.Value.normal;
                if (nearestHit.Value.distance < crawlCheckRadius * 0.9f)
                    return motion + hitNormal * crawlCheckRadius;
                return Vector3.ProjectOnPlane(motion, hitNormal);
            }
            return motion;
        }

        public static bool FindGroundedPosition(this IEntityMovement movement, Vector3 fromPosition, float radius, float findDistance, int layerMask, float resultUpOffsets, out Vector3 result)
        {
            s_findGroundHitPoints.Clear();
            Transform transform = movement.Entity.EntityTransform;
            RaycastHit[] findGroundRaycastHits = ArrayPool<RaycastHit>.Shared.Rent(FIND_GROUND_HIT_ARRAY_LENGTH);
            for (int i = 0; i < s_changePoseRaycastOffsets.Length; ++i)
            {
                Vector3 origin = (s_changePoseRaycastOffsets[i] * radius * (transform.lossyScale.x + transform.lossyScale.z) * 0.5f) + fromPosition + (Vector3.up * findDistance);
                int hitCount = Physics.RaycastNonAlloc(
                    origin, Vector3.down,
                    findGroundRaycastHits, findDistance + 0.5f,
                    layerMask, QueryTriggerInteraction.Ignore);
#if UNITY_EDITOR
                Debug.DrawLine(origin, origin + Vector3.down * (findDistance + 0.5f), Color.red, 30f);
#endif
                for (int j = 0; j < hitCount; ++j)
                {
                    s_findGroundHitPoints.Add(findGroundRaycastHits[j].point);
                }
            }
            ArrayPool<RaycastHit>.Shared.Return(findGroundRaycastHits);
            if (s_findGroundHitPoints.Count <= 0)
            {
                result = new Vector3(fromPosition.x, fromPosition.y + resultUpOffsets, fromPosition.z);
                return false;
            }
            float minDistance = float.MaxValue;
            float tempDistance;
            float resultY = fromPosition.y;
            for (int i = 0; i < s_findGroundHitPoints.Count; ++i)
            {
                tempDistance = Mathf.Abs(s_findGroundHitPoints[i].y - fromPosition.y);
                if (tempDistance < minDistance)
                {
                    minDistance = tempDistance;
                    resultY = s_findGroundHitPoints[i].y;
                }
            }
            result = new Vector3(fromPosition.x, resultY + resultUpOffsets, fromPosition.z);
            return true;
        }

#if UNITY_EDITOR
        public static void DrawCrawlCheckRaycasts(this IEntityMovement movement, int crawlCheckRaycasts, float crawlCheckOffsets, float crawlCheckRadius, Color crawlCheckGizmosColor)
        {
            if (crawlCheckRaycasts <= 0)
                return;
            Transform transform = movement.Entity.EntityTransform;
            Vector3 center = new Vector3(transform.position.x, transform.position.y - crawlCheckOffsets, transform.position.z);
            float[] crawlRaycastDegrees = ArrayPool<float>.Shared.Rent(crawlCheckRaycasts);
            CalculateCrawlRaycastDegrees(crawlCheckRaycasts, ref crawlRaycastDegrees);
            Gizmos.color = crawlCheckGizmosColor;
            for (int i = 0; i < crawlCheckRaycasts; ++i)
            {
                Gizmos.DrawLine(center, center + Quaternion.Euler(0f, crawlRaycastDegrees[i], 0f) * transform.forward * crawlCheckRadius);
            }
            ArrayPool<float>.Shared.Return(crawlRaycastDegrees);
        }
#endif
        #endregion

        #region Transform Player position into Cell and quantized
        public static void GetCellIdAndLocalPosition(Vector3 position, out byte cellId, out Vector3 localposition)
        {
            cellId = DefaultGridManagerComponent.Instance.GetCellId(position);

            localposition = DefaultGridManagerComponent.Instance.GetCellLocalPosition(cellId, position);
        }
        #endregion
    }
}
