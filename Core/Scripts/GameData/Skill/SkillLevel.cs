using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct SkillLevel
    {
        public BaseSkill skill;
        public int level;
    }

    [System.Serializable]
    public struct SkillRandomLevel
    {
        public BaseSkill skill;
        public int minLevel;
        public int maxLevel;
        [Range(0, 1f)]
        public float applyRate;

        public bool Apply(System.Random random)
        {
            return random.NextDouble() <= applyRate;
        }

        public SkillLevel GetRandomedAmount(System.Random random)
        {
            return new SkillLevel()
            {
                skill = skill,
                level = random.RandomInt(minLevel, maxLevel),
            };
        }
    }

    [System.Serializable]
    public struct SkillIncremental
    {
        public BaseSkill skill;
        public IncrementalInt level;
    }
}
