using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetUserUnlockContentsReq : INetSerializable
    {
        public string UserId { get; set; }
        public UnlockableContentType Type { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserId = reader.GetString();
            Type = (UnlockableContentType)reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserId);
            writer.Put((byte)Type);
        }
    }
}
