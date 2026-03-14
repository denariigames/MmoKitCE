using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseLeavePartyMessage : INetSerializable
    {
        public UITextKeys message;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
        }
    }
}
