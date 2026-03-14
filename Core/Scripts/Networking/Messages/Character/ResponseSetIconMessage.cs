using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseSetIconMessage : INetSerializable
    {
        public UITextKeys message;
        public int dataId;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            dataId = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.PutPackedInt(dataId);
        }
    }
}
