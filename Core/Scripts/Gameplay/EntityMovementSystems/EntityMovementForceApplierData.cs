using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct EntityMovementForceApplierData
    {
        [Tooltip("Speed when apply then current speed will be decreased by deceleration * delta time")]
        public float speed;
        [Tooltip("Current speed will be decreased by this value * delta time, you can set this to 0 to make speed not decrease (but you should set duration more than 0)")]
        public float deceleration;
        [Tooltip("If duration <= 0, then it is no duration, it will stop applying when current speed <= 0")]
        public float duration;

        public static EntityMovementForceApplierData CreateDefault()
        {
            return new EntityMovementForceApplierData()
            {
                speed = 20f,
                deceleration = 20f,
                duration = 0f,
            };
        }

        public float CalculateDistance()
        {
            if (duration <= 0f)
            {
                if (deceleration > 0f)
                {
                    return (speed * speed) / (2f * deceleration);
                }
                else
                {
                    // No deceleration — goes infinitely unless you clamp it
                    return Mathf.Infinity;
                }
            }
            else
            {
                return (speed * duration) - (0.5f * deceleration * duration * duration);
            }
        }

        public static float CalculateSpeed(float distance, float duration, float deceleration)
        {
            if (duration <= 0f)
                return 0f; // Invalid duration

            return (distance + (0.5f * deceleration * duration * duration)) / duration;
        }

        public static EntityMovementForceApplierData CreateByDistanceAndDuration(float distance, float duration, float deceleration)
        {
            float speed = CalculateSpeed(distance, duration, deceleration);
            return new EntityMovementForceApplierData()
            {
                speed = speed,
                deceleration = deceleration,
                duration = duration,
            };
        }

        public static float CalculateDuration(float distance, float speed, float deceleration)
        {
            if (deceleration == 0f)
            {
                // No deceleration, simple time = distance / speed
                return speed > 0f ? distance / speed : 0f;
            }

            // Solve: 0.5at^2 - vt + d = 0
            float a = 0.5f * deceleration;
            float b = -speed;
            float c = distance;

            float discriminant = b * b - 4f * a * c;

            if (discriminant < 0f)
                return 0f; // No valid solution

            float sqrt = Mathf.Sqrt(discriminant);
            float t1 = (-b + sqrt) / (2f * a);
            float t2 = (-b - sqrt) / (2f * a);

            // Return the positive and smallest valid root
            if (t1 >= 0f && t2 >= 0f)
                return Mathf.Min(t1, t2);
            else if (t1 >= 0f)
                return t1;
            else if (t2 >= 0f)
                return t2;
            else
                return 0f; // Both roots invalid
        }

        public static EntityMovementForceApplierData CreateByDistanceAndSpeed(float distance, float speed, float deceleration)
        {
            float duration = CalculateDuration(distance, speed, deceleration);
            return new EntityMovementForceApplierData()
            {
                speed = speed,
                deceleration = deceleration,
                duration = duration,
            };
        }
    }
}
