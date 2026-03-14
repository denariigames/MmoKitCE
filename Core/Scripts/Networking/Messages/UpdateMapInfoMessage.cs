using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct UpdateMapInfoMessage : INetSerializable
    {
        public string mapName;
        public string className;

        public void Deserialize(NetDataReader reader)
        {
            mapName = reader.GetString();
            className = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(mapName);
            writer.Put(className);
        }
    }
}
