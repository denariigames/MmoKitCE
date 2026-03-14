using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseReadMailMessage : INetSerializable
    {
        public UITextKeys message;
        public Mail mail;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
                mail = reader.Get(() => new Mail());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
                writer.Put(mail);
        }
    }
}
