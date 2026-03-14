using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseAvailableFramesMessage : INetSerializable
    {
        public UITextKeys message;
        public int[] frameIds;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            frameIds = reader.GetIntArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.PutArray(frameIds);
        }
    }
}
