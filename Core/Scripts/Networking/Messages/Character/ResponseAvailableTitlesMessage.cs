using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseAvailableTitlesMessage : INetSerializable
    {
        public UITextKeys message;
        public int[] titleIds;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            titleIds = reader.GetIntArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.PutArray(titleIds);
        }
    }
}
