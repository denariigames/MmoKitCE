using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct RequestUnlockContentMessage : INetSerializable
    {
        public UnlockableContentType type;
        public int dataId;
        public string options;

        public void Deserialize(NetDataReader reader)
        {
            type = (UnlockableContentType)reader.GetByte();
            dataId = reader.GetPackedInt();
            options = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(dataId);
            writer.Put(options);
        }
    }
}
