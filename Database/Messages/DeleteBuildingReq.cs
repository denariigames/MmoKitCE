using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct DeleteBuildingReq : INetSerializable
    {
        public string ChannelId { get; set; }
        public string MapName { get; set; }
        public string BuildingId { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            ChannelId = reader.GetString();
            MapName = reader.GetString();
            BuildingId = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(ChannelId);
            writer.Put(MapName);
            writer.Put(BuildingId);
        }
    }
}