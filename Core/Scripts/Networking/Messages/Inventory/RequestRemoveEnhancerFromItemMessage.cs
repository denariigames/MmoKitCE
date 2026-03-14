using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestRemoveEnhancerFromItemMessage : INetSerializable
    {
        public InventoryType inventoryType;
        public int index;
        public byte equipSlotIndex;
        public int socketIndex;

        public void Deserialize(NetDataReader reader)
        {
            inventoryType = (InventoryType)reader.GetByte();
            index = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
            socketIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)inventoryType);
            writer.PutPackedInt(index);
            writer.Put(equipSlotIndex);
            writer.PutPackedInt(socketIndex);
        }
    }
}
