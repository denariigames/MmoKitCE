using UnityEngine;

namespace MultiplayerARPG
{
    public interface ICustomAnimationModel
    {
        void PlayCustomAnimation(int id, bool loop);
        void StopCustomAnimation();
        AnimationClip GetCustomAnimationClip(int id);
    }
}
