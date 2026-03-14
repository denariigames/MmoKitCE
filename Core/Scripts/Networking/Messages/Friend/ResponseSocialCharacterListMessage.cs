using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseSocialCharacterListMessage : INetSerializable
    {
        public UITextKeys message;
        public List<SocialCharacterData> characters;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                characters = reader.GetList<SocialCharacterData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.PutList(characters);
        }
    }
}
