using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateUserUnlockContentReq : INetSerializable
    {
        public string UserId { get; set; }
        public UnlockableContent UnlockableContent { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserId = reader.GetString();
            UnlockableContent = reader.Get<UnlockableContent>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserId);
            writer.Put(UnlockableContent);
        }
    }
}
