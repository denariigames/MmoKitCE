using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct ChatMessage : INetSerializable
    {
        public ChatChannel channel;
        public string message;
        public string senderUserId;
        public string senderId;
        public string senderName;
        public string receiverUserId;
        public string receiverId;
        public string receiverName;
        public int guildId;
        public string guildName;
        public int channelId;
        public bool sendByServer;
        public long timestamp;

        public void Deserialize(NetDataReader reader)
        {
            channel = (ChatChannel)reader.GetByte();
            message = reader.GetString();
            senderUserId = reader.GetString();
            senderId = reader.GetString();
            senderName = reader.GetString();
            receiverUserId = reader.GetString();
            receiverId = reader.GetString();
            receiverName = reader.GetString();
            guildId = reader.GetPackedInt();
            if (guildId > 0)
                guildName = reader.GetString();
            if (channel == ChatChannel.Party || channel == ChatChannel.Guild)
                channelId = reader.GetPackedInt();
            sendByServer = reader.GetBool();
            timestamp = reader.GetPackedLong();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)channel);
            writer.Put(message);
            writer.Put(senderUserId);
            writer.Put(senderId);
            writer.Put(senderName);
            writer.Put(receiverUserId);
            writer.Put(receiverId);
            writer.Put(receiverName);
            writer.PutPackedInt(guildId);
            if (guildId > 0)
                writer.Put(guildName);
            if (channel == ChatChannel.Party || channel == ChatChannel.Guild)
                writer.PutPackedInt(channelId);
            writer.Put(sendByServer);
            writer.PutPackedLong(timestamp);
        }
    }
}
