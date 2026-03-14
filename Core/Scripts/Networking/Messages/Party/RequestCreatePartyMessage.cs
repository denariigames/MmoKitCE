using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestCreatePartyMessage : INetSerializable
    {
        public bool shareExp;
        public bool shareItem;

        public void Deserialize(NetDataReader reader)
        {
            shareExp = reader.GetBool();
            shareItem = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(shareExp);
            writer.Put(shareItem);
        }
    }
}
