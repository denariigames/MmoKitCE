using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestDepositGuildGoldMessage : INetSerializable
    {
        public int gold;

        public void Deserialize(NetDataReader reader)
        {
            gold = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(gold);
        }
    }
}
