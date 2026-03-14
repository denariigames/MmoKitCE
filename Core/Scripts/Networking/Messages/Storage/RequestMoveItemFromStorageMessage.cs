using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestMoveItemFromStorageMessage : INetSerializable
    {
        public StorageType storageType;
        public string storageOwnerId;
        public int storageItemIndex;
        public int storageItemAmount;
        public int inventoryItemIndex;
        public InventoryType inventoryType;
        public byte equipSlotIndexOrWeaponSet;

        public void Deserialize(NetDataReader reader)
        {
            storageType = (StorageType)reader.GetByte();
            storageOwnerId = reader.GetString();
            storageItemIndex = reader.GetPackedInt();
            storageItemAmount = reader.GetPackedInt();
            inventoryItemIndex = reader.GetPackedInt();
            inventoryType = (InventoryType)reader.GetByte();
            equipSlotIndexOrWeaponSet = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)storageType);
            writer.Put(storageOwnerId);
            writer.PutPackedInt(storageItemIndex);
            writer.PutPackedInt(storageItemAmount);
            writer.PutPackedInt(inventoryItemIndex);
            writer.Put((byte)inventoryType);
            writer.Put(equipSlotIndexOrWeaponSet);
        }
    }
}
