using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestCashShopBuyMessage : INetSerializable
    {
        public int dataId;
        public CashShopItemCurrencyType currencyType;
        public int amount;

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
            currencyType = (CashShopItemCurrencyType)reader.GetByte();
            amount = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.Put((byte)currencyType);
            writer.PutPackedInt(amount);
        }
    }
}
