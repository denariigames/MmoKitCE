using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestOpenGachaMessage : INetSerializable
    {
        public int dataId;
        public GachaOpenMode openMode;

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
            openMode = (GachaOpenMode)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.Put((byte)openMode);
        }
    }
}
