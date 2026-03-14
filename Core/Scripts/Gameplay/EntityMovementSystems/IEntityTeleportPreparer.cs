using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IEntityTeleportPreparer
    {
        bool IsPreparingToTeleport { get; }
        Vector3 PrepareTeleportPosition { get; }
        UniTask PrepareToTeleport(Vector3 position, Quaternion rotation);
    }
}