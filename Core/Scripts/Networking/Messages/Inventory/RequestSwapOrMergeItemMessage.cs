using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSwapOrMergeItemMessage : INetSerializable
    {
        public int fromIndex;
        public int toIndex;

        public void Deserialize(NetDataReader reader)
        {
            fromIndex = reader.GetPackedInt();
            toIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(fromIndex);
            writer.PutPackedInt(toIndex);
        }
    }
}
