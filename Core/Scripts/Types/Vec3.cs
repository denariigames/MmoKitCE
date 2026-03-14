using UnityEngine;

namespace MultiplayerARPG
{
    public partial struct Vec3
    {
        public static implicit operator Vec3(Vector3 value) { return new Vec3(value); }
        public static implicit operator Vector3(Vec3 value) { return new Vector3(value.x, value.y, value.z); }

        public Vec3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }
    }
}
