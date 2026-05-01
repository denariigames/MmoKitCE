using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct GuildRoleData : INetSerializable
    {
        public string roleName;
        public bool canInvite;
        public bool canKick;
        public bool canUseStorage;
        public byte shareExpPercentage;

        public void Deserialize(NetDataReader reader)
        {
            roleName = reader.GetString();
            canInvite = reader.GetBool();
            canKick = reader.GetBool();
            canUseStorage = reader.GetBool();
            shareExpPercentage = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(roleName);
            writer.Put(canInvite);
            writer.Put(canKick);
            writer.Put(canUseStorage);
            writer.Put(shareExpPercentage);
        }
    }
}
