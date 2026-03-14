using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSendGuildInvitationMessage : INetSerializable
    {
        public string inviteeId;

        public void Deserialize(NetDataReader reader)
        {
            inviteeId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(inviteeId);
        }
    }
}
