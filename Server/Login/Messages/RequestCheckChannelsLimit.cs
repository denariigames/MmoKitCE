using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
    public struct RequestCheckChannelsLimit : INetSerializable
    {
        public string channelId;

        public void Deserialize(NetDataReader reader)
        {
            channelId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(channelId);
        }
    }
}
