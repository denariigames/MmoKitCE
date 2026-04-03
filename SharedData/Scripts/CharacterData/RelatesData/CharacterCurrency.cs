using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct CharacterCurrency : INetSerializable
    {
        public static readonly CharacterCurrency Empty = new CharacterCurrency();
        public int dataId;
        public int amount;

        public CharacterCurrency Clone()
        {
            return new CharacterCurrency()
            {
                dataId = dataId,
                amount = amount,
            };
        }

        public static CharacterCurrency Create(int dataId, int amount = 0)
        {
            return new CharacterCurrency()
            {
                dataId = dataId,
                amount = amount,
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.PutPackedInt(amount);
        }

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
            amount = reader.GetPackedInt();
        }
    }
}
