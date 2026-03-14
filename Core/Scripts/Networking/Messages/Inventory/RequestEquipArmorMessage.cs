using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestEquipArmorMessage : INetSerializable
    {
        public int nonEquipIndex;
        public byte equipSlotIndex;

        public void Deserialize(NetDataReader reader)
        {
            nonEquipIndex = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(nonEquipIndex);
            writer.Put(equipSlotIndex);
        }
    }
}
