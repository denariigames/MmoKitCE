using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseGetFriendRequestsMessage : INetSerializable
    {
        public UITextKeys message;
        public List<SocialCharacterData> friendRequests;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                friendRequests = reader.GetList<SocialCharacterData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.PutList(friendRequests);
        }
    }
}