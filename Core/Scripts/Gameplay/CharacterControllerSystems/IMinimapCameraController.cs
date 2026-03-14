using UnityEngine;

namespace MultiplayerARPG
{
    public interface IMinimapCameraController
    {
        GameObject gameObject { get; }
        bool enabled { get; }
        Camera Camera { get; }
        Transform CameraTransform { get; }
        Transform FollowingEntityTransform { get; set; }
        Transform FollowingGameplayCameraTransform { get; set; }
        void Init();
        void Setup(BasePlayerCharacterEntity characterEntity);
        void Desetup(BasePlayerCharacterEntity characterEntity);
    }
}
