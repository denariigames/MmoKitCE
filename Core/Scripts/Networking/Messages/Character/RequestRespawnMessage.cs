using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestRespawnMessage : INetSerializable
    {
        public int option;

        public void Deserialize(NetDataReader reader)
        {
            option = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(option);
        }
    }
}
