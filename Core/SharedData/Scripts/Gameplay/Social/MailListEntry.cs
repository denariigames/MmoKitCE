using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct MailListEntry : INetSerializable
    {
        public string Id { get; set; }
        public string SenderName { get; set; }
        public string Title { get; set; }
        public bool IsRead { get; set; }
        public bool IsClaim { get; set; }
        public long SentTimestamp { get; set; }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id);
            writer.Put(SenderName);
            writer.Put(Title);
            writer.Put(IsRead);
            writer.Put(IsClaim);
            writer.PutPackedLong(SentTimestamp);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = reader.GetString();
            SenderName = reader.GetString();
            Title = reader.GetString();
            IsRead = reader.GetBool();
            IsClaim = reader.GetBool();
            SentTimestamp = reader.GetPackedLong();
        }
    }
}
