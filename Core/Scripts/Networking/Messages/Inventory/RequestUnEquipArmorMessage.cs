using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestUnEquipArmorMessage : INetSerializable
    {
        public int equipIndex;
        public int nonEquipIndex;

        public void Deserialize(NetDataReader reader)
        {
            equipIndex = reader.GetPackedInt();
            nonEquipIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(equipIndex);
            writer.PutPackedInt(nonEquipIndex);
        }
    }
}
