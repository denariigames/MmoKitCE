using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestMoveItemToStorageMessage : INetSerializable
    {
        public StorageType storageType;
        public string storageOwnerId;
        public int inventoryItemIndex;
        public int inventoryItemAmount;
        public int storageItemIndex;
        public InventoryType inventoryType;
        public byte equipSlotIndexOrWeaponSet;

        public void Deserialize(NetDataReader reader)
        {
            storageType = (StorageType)reader.GetByte();
            storageOwnerId = reader.GetString();
            inventoryItemIndex = reader.GetPackedInt();
            inventoryItemAmount = reader.GetPackedInt();
            storageItemIndex = reader.GetPackedInt();
            inventoryType = (InventoryType)reader.GetByte();
            equipSlotIndexOrWeaponSet = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)storageType);
            writer.Put(storageOwnerId);
            writer.PutPackedInt(inventoryItemIndex);
            writer.PutPackedInt(inventoryItemAmount);
            writer.PutPackedInt(storageItemIndex);
            writer.Put((byte)inventoryType);
            writer.Put(equipSlotIndexOrWeaponSet);
        }
    }
}
