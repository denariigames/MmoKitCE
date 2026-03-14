using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestChangeGuildOptionsMessage : INetSerializable
    {
        public string options;

        public void Deserialize(NetDataReader reader)
        {
            options = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(options);
        }
    }
}