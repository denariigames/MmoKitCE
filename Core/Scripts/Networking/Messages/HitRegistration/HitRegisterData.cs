using LiteNetLibManager;
using LiteNetLib.Utils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct HitRegisterData : INetSerializable
    {
        public int SimulateSeed { get; set; }
        public byte TriggerIndex { get; set; }
        public byte SpreadIndex { get; set; }
        public long LaunchTimestamp { get; set; }
        public Vector3 Origin { get; set; }
        public DirectionVector3 Direction { get; set; }
        public long HitTimestamp { get; set; }
        public uint HitObjectId { get; set; }
        public byte HitBoxIndex { get; set; }
        public Vector3 HitOrigin { get; set; }
        public Vector3? HitDestination { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(SimulateSeed);
            writer.Put(TriggerIndex);
            writer.Put(SpreadIndex);
            writer.PutPackedLong(LaunchTimestamp);
            writer.PutVector3(Origin);
            writer.Put(Direction);
            writer.PutPackedLong(HitTimestamp);
            writer.PutPackedUInt(HitObjectId);
            writer.Put(HitBoxIndex);
            writer.PutVector3(HitOrigin);
            writer.Put(HitDestination.HasValue);
            if (HitDestination.HasValue)
                writer.PutVector3(HitDestination.Value);
        }

        public void Deserialize(NetDataReader reader)
        {
            SimulateSeed = reader.GetPackedInt();
            TriggerIndex = reader.GetByte();
            SpreadIndex = reader.GetByte();
            LaunchTimestamp = reader.GetPackedLong();
            Origin = reader.GetVector3();
            Direction = reader.Get<DirectionVector3>();
            HitTimestamp = reader.GetPackedLong();
            HitObjectId = reader.GetPackedUInt();
            HitBoxIndex = reader.GetByte();
            HitOrigin = reader.GetVector3();
            if (reader.GetBool())
                HitDestination = reader.GetVector3();
        }

        public string GetHitId()
        {
            return HitRegistrationUtils.MakeValidateId(TriggerIndex, SpreadIndex);
        }

        public string GetHitObjectId()
        {
            return HitRegistrationUtils.MakeHitObjectId(TriggerIndex, SpreadIndex, HitObjectId);
        }
    }
}
