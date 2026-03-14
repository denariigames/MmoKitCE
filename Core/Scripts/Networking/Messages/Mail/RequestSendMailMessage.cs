using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSendMailMessage : INetSerializable
    {
        public string receiverName;
        public string title;
        public string content;
        public int gold;

        public void Deserialize(NetDataReader reader)
        {
            receiverName = reader.GetString();
            title = reader.GetString();
            content = reader.GetString();
            gold = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(receiverName);
            writer.Put(title);
            writer.Put(content);
            writer.Put(gold);
        }
    }
}
