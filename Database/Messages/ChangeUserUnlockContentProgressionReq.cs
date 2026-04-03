using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct ChangeUserUnlockContentProgressionReq : INetSerializable
    {
        public string UserId { get; set; }
        public UnlockableContentType Type { get; set; }
        public int DataId { get; set; }
        public int Amount { get; set; }
        public bool Unlocked { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            UserId = reader.GetString();
            Type = (UnlockableContentType)reader.GetByte();
            DataId = reader.GetInt();
            Amount = reader.GetInt();
            Unlocked = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserId);
            writer.Put((byte)Type);
            writer.Put(DataId);
            writer.Put(Amount);
            writer.Put(Unlocked);
        }
    }
}
