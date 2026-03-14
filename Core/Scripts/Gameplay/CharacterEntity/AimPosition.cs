using LiteNetLib.Utils;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct AimPosition : INetSerializable
    {
        public AimPositionType type;
        public Vector3 position;
        public DirectionVector3 direction;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            if (type != AimPositionType.None)
                writer.PutVector3(position);
            if (type == AimPositionType.Direction)
                writer.Put(direction);
        }

        public void Deserialize(NetDataReader reader)
        {
            type = (AimPositionType)reader.GetByte();
            if (type != AimPositionType.None)
                position = reader.GetVector3();
            if (type == AimPositionType.Direction)
                direction = reader.Get<DirectionVector3>();
        }

        public static AimPosition CreatePosition(Vector3 position)
        {
            return new AimPosition()
            {
                type = AimPositionType.Position,
                position = position,
            };
        }

        public static AimPosition CreateDirection(Vector3 position, Vector3 direction)
        {
            return new AimPosition()
            {
                type = AimPositionType.Direction,
                position = position,
                direction = direction,
            };
        }

        public override string ToString()
        {
            return $"{type} {position} {(Vector3)direction}";
        }
    }

    [System.Serializable]
    public class SyncFieldAimPosition : LiteNetLibSyncField<AimPosition>
    {
        protected override bool IsValueChanged(AimPosition oldValue, AimPosition newValue)
        {
            return true;
        }
    }
}
