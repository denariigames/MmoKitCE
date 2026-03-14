using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestOpenStorageMessage : INetSerializable
    {
        public StorageType storageType;

        public void Deserialize(NetDataReader reader)
        {
            storageType = (StorageType)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)storageType);
        }
    }
}
