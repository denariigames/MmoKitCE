using UnityEngine;

namespace MultiplayerARPG
{
    public interface ICraftingQueueSource
    {
        SyncListCraftingQueueItem QueueItems { get; }
        int MaxQueueSize { get; }
        float CraftingDistance { get; }
        bool PublicQueue { get; }
        bool CanCraft { get; }
        float TimeCounter { get; set; }
        int SourceId { get; }
        uint ObjectId { get; }
        Transform transform { get; }
    }
}
