using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct UpdateServerInfoMessage : INetSerializable
    {
        public string channelId;
        public string channelTitle;
        public string channelDescription;

        public void Deserialize(NetDataReader reader)
        {
            channelId = reader.GetString();
            channelTitle = reader.GetString();
            channelDescription = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(channelId);
            writer.Put(channelTitle);
            writer.Put(channelDescription);
        }
    }
}
