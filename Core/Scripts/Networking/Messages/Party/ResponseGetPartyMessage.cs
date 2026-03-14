using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponseGetPartyMessage : INetSerializable
    {
        public PartyData party;

        public void Deserialize(NetDataReader reader)
        {
            bool notNull = reader.GetBool();
            if (notNull)
                party = reader.Get(() => new PartyData());
        }

        public void Serialize(NetDataWriter writer)
        {
            bool notNull = party != null;
            writer.Put(notNull);
            if (notNull)
                writer.Put(party);
        }
    }
}
