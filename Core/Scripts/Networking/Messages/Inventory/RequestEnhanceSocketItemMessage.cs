using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestEnhanceSocketItemMessage : INetSerializable
    {
        public InventoryType inventoryType;
        public int index;
        public byte equipSlotIndex;
        public int enhancerId;
        public int socketIndex;

        public void Deserialize(NetDataReader reader)
        {
            inventoryType = (InventoryType)reader.GetByte();
            index = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
            enhancerId = reader.GetPackedInt();
            socketIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)inventoryType);
            writer.PutPackedInt(index);
            writer.Put(equipSlotIndex);
            writer.PutPackedInt(enhancerId);
            writer.PutPackedInt(socketIndex);
        }
    }
}
