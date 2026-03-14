using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct RequestUnlockContentProgressionMessage : INetSerializable
    {
        public UnlockableContentType type;
        public int dataId;

        public void Deserialize(NetDataReader reader)
        {
            type = (UnlockableContentType)reader.GetByte();
            dataId = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(dataId);
        }
    }
}
