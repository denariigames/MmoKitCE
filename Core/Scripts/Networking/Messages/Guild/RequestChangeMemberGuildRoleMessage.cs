using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestChangeMemberGuildRoleMessage : INetSerializable
    {
        public string memberId;
        public byte guildRole;

        public void Deserialize(NetDataReader reader)
        {
            memberId = reader.GetString();
            guildRole = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(memberId);
            writer.Put(guildRole);
        }
    }
}
