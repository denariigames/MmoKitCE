using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseFriendRequestNotificationMessage : INetSerializable
    {
        public UITextKeys message;
        public int notificationCount;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                notificationCount = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.PutPackedInt(notificationCount);
        }
    }
}
