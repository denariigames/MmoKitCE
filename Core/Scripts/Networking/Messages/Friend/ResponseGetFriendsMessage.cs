using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseGetFriendsMessage : INetSerializable
    {
        public UITextKeys message;
        public List<SocialCharacterData> friends;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                friends = reader.GetList<SocialCharacterData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.PutList(friends);
        }
    }
}