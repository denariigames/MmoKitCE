using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct PlayerSkill
    {
        public BaseSkill skill;
        // TODO: This one is deprecating, use `skillLevel` instead
        [HideInInspector]
        public int level;
        public IncrementalInt skillLevel;
    }
}
