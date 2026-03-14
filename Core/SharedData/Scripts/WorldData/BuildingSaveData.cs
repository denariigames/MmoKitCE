using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class BuildingSaveData : IBuildingSaveData, INetSerializable
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public int EntityId { get; set; }
        public int CurrentHp { get; set; }
        public float RemainsLifeTime { get; set; }
        public bool IsLocked { get; set; }
        public string LockPassword { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string ExtraData { get; set; }
        public bool IsSceneObject { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            this.DeserializeBuildingSaveData(reader);
        }

        public void Serialize(NetDataWriter writer)
        {
            this.SerializeBuildingSaveData(writer);
        }
    }
}
