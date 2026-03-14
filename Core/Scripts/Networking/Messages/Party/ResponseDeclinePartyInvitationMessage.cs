using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseDeclinePartyInvitationMessage : INetSerializable
    {
        public UITextKeys message;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
        }
    }
}
