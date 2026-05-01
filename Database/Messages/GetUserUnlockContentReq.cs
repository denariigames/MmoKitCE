using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetUserUnlockContentReq : INetSerializable
    {
        public string UserId { get; set; }
        public UnlockableContentType Type { get; set; }
        public int DataId { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserId = reader.GetString();
            Type = (UnlockableContentType)reader.GetByte();
            DataId = reader.GetInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserId);
            writer.Put((byte)Type);
            writer.Put(DataId);
        }
    }
}
