using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct StartVendingItem : INetSerializable
    {
        public string id;
        public int amount;
        public int price;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(id);
            writer.PutPackedInt(amount);
            writer.PutPackedInt(price);
        }

        public void Deserialize(NetDataReader reader)
        {
            id = reader.GetString();
            amount = reader.GetPackedInt();
            price = reader.GetPackedInt();
        }
    }
}