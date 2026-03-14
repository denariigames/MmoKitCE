using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct PartyInvitationData : INetSerializable
    {
        public string InviterId { get; set; }
        public string InviterName { get; set; }
        public int InviterLevel { get; set; }
        public int PartyId { get; set; }
        public bool ShareExp { get; set; }
        public bool ShareItem { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            InviterId = reader.GetString();
            InviterName = reader.GetString();
            InviterLevel = reader.GetPackedInt();
            PartyId = reader.GetPackedInt();
            ShareExp = reader.GetBool();
            ShareItem = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(InviterId);
            writer.Put(InviterName);
            writer.PutPackedInt(InviterLevel);
            writer.PutPackedInt(PartyId);
            writer.Put(ShareExp);
            writer.Put(ShareItem);
        }
    }
}
