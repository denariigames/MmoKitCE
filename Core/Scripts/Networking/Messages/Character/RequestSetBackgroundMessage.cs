using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSetBackgroundMessage : INetSerializable
    {
        public int dataId;

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
        }
    }
}
