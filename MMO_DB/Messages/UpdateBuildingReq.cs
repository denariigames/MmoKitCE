using LiteNetLib.Utils;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateBuildingReq : INetSerializable
    {
        public TransactionUpdateBuildingState State { get; set; }
        public string ChannelId { get; set; }
        public string MapName { get; set; }
        public BuildingSaveData BuildingData { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            State = (TransactionUpdateBuildingState)reader.GetPackedUInt();
            ChannelId = reader.GetString();
            MapName = reader.GetString();
            BuildingData = reader.Get(() => new BuildingSaveData());
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUInt((uint)State);
            writer.Put(ChannelId);
            writer.Put(MapName);
            writer.Put(BuildingData);
        }
    }
}