using Cysharp.Text;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class BuffRemoval
    {
        [Tooltip("Source of buff (item level, skill level, status effect level and so on).")]
        public BuffSourceData source = new BuffSourceData();
        [Tooltip("Chance to remove buff will be calculated by buff's source level (item level, skill level, status effect level and so on).")]
        public IncrementalFloat removalChance;
        [Min(0f)]
        [Tooltip("If removal chance is `1.5`, it will `100%` resist remove level `1` and `50%` resist remove level `2`. Set it to `0` to no limit.")]
        public float maxChance = 1f;
        [Range(0f, 1f)]
        [Tooltip("If value is `[0.8, 0.5, 0.25]`, and your removal chance is `2.15`, it will have chance `80%` to remove buff level `1`, `50%` to remove level `2`, and `15%` to remove level `3`.")]
        public float[] maxChanceEachLevels = new float[0];

        public string Title => source.Title;
        public string Description => source.Description;
#if UNITY_EDITOR || !UNITY_SERVER
        public async UniTask<Sprite> GetIcon()
        {
            return await source.GetIcon();
        }
#endif

        public bool IsValid()
        {
            return source.IsValid();
        }

        public string GetId()
        {
            return source.GetId();
        }

        public override int GetHashCode()
        {
            return source.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return source.ToString();
        }

        public float GetChanceByLevel(float totalChance, int level)
        {
            if (maxChance > 0f && totalChance > maxChance)
                totalChance = maxChance;
            float chance = totalChance / level;
            if (chance > 1f)
                chance = 1f;
            if (maxChanceEachLevels == null || maxChanceEachLevels.Length == 0)
                return chance;
            int index = level - 1;
            if (index >= 0)
            {
                if (index < maxChanceEachLevels.Length)
                    chance = Mathf.Min(chance, maxChanceEachLevels[index]);
                else
                    chance = Mathf.Min(chance, maxChanceEachLevels[maxChanceEachLevels.Length - 1]);
            }
            return chance;
        }

        public bool RandomRemoveOccurs(float totalChance, int level)
        {
            float chance = GetChanceByLevel(totalChance, level);
            return chance > 0f && Random.value <= chance;
        }

        public string GetChanceEntriesText(float totalChance, string format, string separator = ",")
        {
            if (totalChance > maxChance)
                totalChance = maxChance;
            List<string> entry = new List<string>();
            for (int i = 0; i < totalChance; ++i)
            {
                int level = i + 1;
                float chance = GetChanceByLevel(totalChance, level);
                entry.Add(ZString.Format(
                        format,
                        level.ToString("N0"),
                        (chance * 100f).ToString("N2")));
            }
            return string.Join(separator, entry);
        }
    }
}