using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct ResponseCashShopBuyMessage : INetSerializable
    {
        public UITextKeys message;
        public int dataId;
        public int rewardGold;
        public List<RewardedItem> rewardItems;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            if (!message.IsError())
            {
                dataId = reader.GetPackedInt();
                rewardGold = reader.GetPackedInt();
                rewardItems = reader.GetList<RewardedItem>();
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            if (!message.IsError())
            {
                writer.PutPackedInt(dataId);
                writer.PutPackedInt(rewardGold);
                writer.PutList(rewardItems);
            }
        }
    }
}
