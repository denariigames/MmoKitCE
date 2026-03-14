using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct SimulateActionTriggerData : INetSerializable
    {
        public int simulateSeed;
        public byte triggerIndex;
        public uint targetObjectId;
        public AimPosition aimPosition;

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(simulateSeed);
            writer.Put(triggerIndex);
            writer.PutPackedUInt(targetObjectId);
            writer.Put(aimPosition);
        }

        public void Deserialize(NetDataReader reader)
        {
            simulateSeed = reader.GetPackedInt();
            triggerIndex = reader.GetByte();
            targetObjectId = reader.GetPackedUInt();
            aimPosition = reader.Get<AimPosition>();
        }
    }
}
