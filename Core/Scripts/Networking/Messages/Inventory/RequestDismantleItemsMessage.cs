using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestDismantleItemsMessage : INetSerializable
    {
        public int[] selectedIndexes;

        public void Deserialize(NetDataReader reader)
        {
            selectedIndexes = reader.GetIntArray();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutArray(selectedIndexes);
        }
    }
}
