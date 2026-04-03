namespace MultiplayerARPG
{
    public partial interface IBuildingSaveData
    {
        string Id { get; set; }
        string ParentId { get; set; }
        int EntityId { get; set; }
        int CurrentHp { get; set; }
        float RemainsLifeTime { get; set; }
        bool IsLocked { get; set; }
        string LockPassword { get; set; }
        Vec3 Position { get; set; }
        Vec3 Rotation { get; set; }
        string CreatorId { get; set; }
        string CreatorName { get; set; }
        string ExtraData { get; set; }
        bool IsSceneObject { get; set; }
    }
}
