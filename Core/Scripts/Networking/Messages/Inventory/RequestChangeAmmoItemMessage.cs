using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestChangeAmmoItemMessage : INetSerializable
    {
        public InventoryType inventoryType;
        public int index;
        public byte equipSlotIndex;
        public string ammoItemId;

        public void Deserialize(NetDataReader reader)
        {
            inventoryType = (InventoryType)reader.GetByte();
            index = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
            ammoItemId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)inventoryType);
            writer.PutPackedInt(index);
            writer.Put(equipSlotIndex);
            writer.Put(ammoItemId);
        }
    }
}
