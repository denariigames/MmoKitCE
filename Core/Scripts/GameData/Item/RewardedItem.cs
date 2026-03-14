using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct RewardedItem : INetSerializable
    {
        public BaseItem item;
        public int level;
        public int amount;
        public int randomSeed;

        public void SetItemByDataId(int dataId)
        {
            item = GameInstance.Items.ContainsKey(dataId) ? GameInstance.Items[dataId] : null;
        }

        public void Deserialize(NetDataReader reader)
        {
            SetItemByDataId(reader.GetPackedInt());
            level = reader.GetPackedInt();
            amount = reader.GetPackedInt();
            randomSeed = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(item != null ? item.DataId : 0);
            writer.PutPackedInt(level);
            writer.PutPackedInt(amount);
            writer.PutPackedInt(randomSeed);
        }
    }
}
