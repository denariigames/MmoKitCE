using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Animation Clip 2D", menuName = "Animation Clip 2D")]
    public class AnimationClip2D : ScriptableObject
    {
        public Sprite[] frames;
        public float framesPerSec = 5;
        public bool loop = true;
        public bool flipX;
        public bool flipY;

        public float length
        {
            get { return frames.Length / framesPerSec; }
            set { framesPerSec = frames.Length / value; }
        }
    }
}
