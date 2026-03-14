using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSellItemMessage : INetSerializable
    {
        public InventoryType inventoryType;
        public int index;
        public byte equipSlotIndex;
        public int amount;

        public void Deserialize(NetDataReader reader)
        {
            inventoryType = (InventoryType)reader.GetByte();
            index = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
            amount = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)inventoryType);
            writer.PutPackedInt(index);
            writer.Put(equipSlotIndex);
            writer.PutPackedInt(amount);
        }
    }
}
