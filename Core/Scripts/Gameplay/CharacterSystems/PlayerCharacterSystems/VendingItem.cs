using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class VendingItem : INetSerializable
    {
        public CharacterItem item;
        public int price;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(item);
            writer.PutPackedInt(price);
        }

        public void Deserialize(NetDataReader reader)
        {
            item = reader.Get<CharacterItem>();
            price = reader.GetPackedInt();
        }
    }
}