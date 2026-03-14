using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestRemoveAmmoFromItemMessage : INetSerializable
    {
        public InventoryType inventoryType;
        public int index;
        public byte equipSlotIndex;

        public void Deserialize(NetDataReader reader)
        {
            inventoryType = (InventoryType)reader.GetByte();
            index = reader.GetPackedInt();
            equipSlotIndex = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)inventoryType);
            writer.PutPackedInt(index);
            writer.Put(equipSlotIndex);
        }
    }
}
