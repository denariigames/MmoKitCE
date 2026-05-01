using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UserUnlockContentResp : INetSerializable
    {
        public UnlockableContent UnlockableContent { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UnlockableContent = reader.Get<UnlockableContent>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UnlockableContent);
        }
    }
}