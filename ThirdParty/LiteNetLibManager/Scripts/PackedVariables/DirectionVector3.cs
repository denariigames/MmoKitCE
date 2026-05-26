//bad values are forced back into the safe range
using UnityEngine;
using LiteNetLib.Utils;

namespace LiteNetLibManager
{
    public struct DirectionVector3 : INetSerializable
    {
        public static implicit operator DirectionVector3(Vector3 value) => new DirectionVector3(value);
        public static implicit operator Vector3(DirectionVector3 value) => value.ToVector3();

        private const float MULTIPLIER = 100f;
        private const float INV_MULTIPLIER = 1f / MULTIPLIER;

        public Vector3 ToVector3() => new Vector3(x * INV_MULTIPLIER, y * INV_MULTIPLIER, z * INV_MULTIPLIER);

        public sbyte x;
        public sbyte y;
        public sbyte z;

        public DirectionVector3(Vector3 vector3)
        {
            x = PackDirectionComponent(vector3.x);
            y = PackDirectionComponent(vector3.y);
            z = PackDirectionComponent(vector3.z);
        }

        private static sbyte PackDirectionComponent(float value)
        {
            value = Mathf.Clamp(value, -1f, 1f);
            return (sbyte)Mathf.RoundToInt(value * MULTIPLIER);
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
        }

        public void Deserialize(NetDataReader reader)
        {
            x = reader.GetSByte();
            y = reader.GetSByte();
            z = reader.GetSByte();
        }

        public override string ToString()
        {
            return ToVector3().ToString();
        }
    }
}
