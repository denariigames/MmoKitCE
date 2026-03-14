using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSwapOrMergeStorageItemMessage : INetSerializable
    {
        public StorageType storageType;
        public string storageOwnerId;
        public int fromIndex;
        public int toIndex;

        public void Deserialize(NetDataReader reader)
        {
            storageType = (StorageType)reader.GetByte();
            storageOwnerId = reader.GetString();
            fromIndex = reader.GetPackedInt();
            toIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)storageType);
            writer.Put(storageOwnerId);
            writer.PutPackedInt(fromIndex);
            writer.PutPackedInt(toIndex);
        }
    }
}
