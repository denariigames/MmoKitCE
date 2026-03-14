using UnityEngine;

namespace MultiplayerARPG
{
    public static class GameplayUtils
    {
        public static Vector3 CursorWorldPosition(Camera camera, Vector3 cursorPosition, float distance = 100f)
        {
            return CursorWorldPosition(camera, cursorPosition, distance, GameInstance.Singleton.GetTargetLayerMask());
        }

        public static Vector3 CursorWorldPosition(Camera camera, Vector3 cursorPosition, float distance, int layerMask)
        {
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension3D)
            {
                RaycastHit tempHit;
                if (Physics.Raycast(camera.ScreenPointToRay(cursorPosition), out tempHit, distance, layerMask))
                {
                    return tempHit.point;
                }
            }
            return camera.ScreenToWorldPoint(cursorPosition);
        }

        public static Vector3 ClampPosition(Vector3 centerPosition, Vector3 validatingPosition, float distance)
        {
            Vector3 offset = validatingPosition - centerPosition;
            return centerPosition + Vector3.ClampMagnitude(offset, distance);
        }

        public static Vector3 ClampPositionXZ(Vector3 centerPosition, Vector3 validatingPosition, float distance)
        {
            float y = centerPosition.y;
            centerPosition.y = 0;
            validatingPosition.y = 0;
            Vector3 offset = validatingPosition - centerPosition;
            return centerPosition + Vector3.ClampMagnitude(offset, distance) + (Vector3.up * y);
        }

        public static Vector3 GetDirectionByAxes(Transform cameraTransform, float xAxis, float yAxis)
        {
            Vector3 aimDirection = Vector3.zero;
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension3D)
            {
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();
                aimDirection += forward * yAxis;
                aimDirection += right * xAxis;
                // normalize input if it exceeds 1 in combined length:
                if (aimDirection.sqrMagnitude > 1)
                    aimDirection.Normalize();
                return aimDirection;
            }
            else
            {
                return new Vector2(xAxis, yAxis);
            }
        }

        public static DirectionType2D GetDirectionTypeByVector2(Vector2 direction)
        {
            float angle = Vector2.SignedAngle(direction, Vector2.down);
            if (angle < 0)
                angle += 360f;
            if (angle > 22.5f && angle <= 67.5f)
                return DirectionType2D.DownLeft;
            else if (angle > 67.5f && angle <= 112.5f)
                return DirectionType2D.Left;
            else if (angle > 112.5f && angle <= 157.5f)
                return DirectionType2D.UpLeft;
            else if (angle > 157.5f && angle <= 202.5f)
                return DirectionType2D.Up;
            else if (angle > 202.5f && angle <= 247.5f)
                return DirectionType2D.UpRight;
            else if (angle > 247.5f && angle <= 292.5f)
                return DirectionType2D.Right;
            else if (angle > 292.5f && angle <= 337.5f)
                return DirectionType2D.DownRight;
            return DirectionType2D.Down;
        }

        public static MovementState GetMovementStateByDirection(Vector3 moveDirection, Ladder ladder)
        {
            float angle = Vector3.SignedAngle(moveDirection, ladder.ForwardWithYAngleOffsets, ladder.Up);
            return Mathf.Abs(angle) > 90f ? MovementState.Up : MovementState.Down;
        }

        public static MovementState GetMovementStateByDirection(Vector3 moveDirection, Vector3 entityForward)
        {
            float angle = Vector3.SignedAngle(moveDirection, entityForward, Vector3.up);
            if (angle >= -30 && angle <= 30)
            {
                return MovementState.Forward;
            }
            else if (angle >= 30 && angle <= 60)
            {
                return MovementState.Forward | MovementState.Left;
            }
            else if (angle >= 60 && angle <= 120)
            {
                return MovementState.Left;
            }
            else if (angle >= 120 && angle <= 150)
            {
                return MovementState.Backward | MovementState.Left;
            }
            else if (angle >= 150 || angle <= -150)
            {
                return MovementState.Backward;
            }
            else if (angle <= -120 && angle >= -150)
            {
                return MovementState.Backward | MovementState.Right;
            }
            else if (angle <= -60 && angle >= -120)
            {
                return MovementState.Right;
            }
            else if (angle <= -30 && angle >= -60)
            {
                return MovementState.Forward | MovementState.Right;
            }
            return MovementState.None;
        }

        public static MovementState GetStraightlyMovementStateByDirection(Vector3 moveDirection, Vector3 entityForward)
        {
            float angle = Vector3.SignedAngle(moveDirection, entityForward, Vector3.up);
            if (angle >= -60 && angle <= 60)
            {
                return MovementState.Forward;
            }
            else if (angle >= 60 && angle <= 120)
            {
                return MovementState.Left;
            }
            else if (angle >= 120 || angle <= -120)
            {
                return MovementState.Backward;
            }
            else if (angle <= -60 && angle >= -120)
            {
                return MovementState.Right;
            }
            return MovementState.None;
        }

        public static Bounds MakeLocalBoundsByCollider(Transform transform)
        {
            Bounds result = new Bounds();
            if (GameInstance.Singleton.DimensionType == DimensionType.Dimension3D)
            {
                Collider col = transform.GetComponent<Collider>();
                if (col != null)
                {
                    result = col.bounds;
                    result.center = result.center - transform.position;
                }
            }
            else
            {
                Collider2D col2d = transform.GetComponent<Collider2D>();
                if (col2d != null)
                {
                    result = col2d.bounds;
                    result.center = result.center - transform.position;
                }
            }
            return result;
        }

        public static float BoundsDistance(Bounds a, Bounds b)
        {
            return Vector3.Distance(a.ClosestPoint(b.center), b.ClosestPoint(a.center));
        }

        public static float BoundsDistance(Bounds a, Vector3 b)
        {
            return Vector3.Distance(a.ClosestPoint(b), b);
        }

        public static float GetPitchBetween(Vector3 origin, Vector3 target)
        {
            return GetPitchByDirection((target - origin).normalized);
        }

        public static float GetPitchByDirection(Vector3 direction)
        {
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
    }
}
