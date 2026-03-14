using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct DamageHitObjectInfo : INetSerializable
    {
        public uint ObjectId { get; set; }
        public int HitBoxIndex { get; set; }

        public override int GetHashCode()
        {
            return ObjectId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUInt(ObjectId);
            writer.PutPackedInt(HitBoxIndex);
        }

        public void Deserialize(NetDataReader reader)
        {
            ObjectId = reader.GetPackedUInt();
            HitBoxIndex = reader.GetPackedInt();
        }
    }
}
