using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.EXP_TABLE_FILE, menuName = GameDataMenuConsts.EXP_TABLE_MENU, order = GameDataMenuConsts.EXP_TABLE_ORDER)]
    public class ExpTable : ScriptableObject
    {
        public int[] expTree = new int[0];

        public int GetNextLevelExp(int level)
        {
            if (level <= 0)
                return 0;
            if (level > expTree.Length)
                return 0;
            return expTree[level - 1];
        }

        public void GetProperCurrentByNextLevelExp(int currentLevel, int currentExp, out int properCurrentExp, out int properNextLevelExp)
        {
            properCurrentExp = currentExp;
            properNextLevelExp = GetNextLevelExp(currentLevel);
            if (currentLevel >= expTree.Length && currentLevel - 2 > 0 && currentLevel - 2 < expTree.Length)
            {
                int maxExp = expTree[currentLevel - 2];
                properCurrentExp = maxExp;
                properNextLevelExp = maxExp;
            }
            if (properCurrentExp > properNextLevelExp)
                properCurrentExp = properNextLevelExp;
        }

#if UNITY_EDITOR
        [Header("Exp calculator")]
        public int maxLevel;
        public Int32GraphCalculator expCalculator;
        [InspectorButton(nameof(CalculateExp))]
        public bool calculateExp;

        public void CalculateExp()
        {
            int[] expTree = new int[maxLevel - 1];
            for (int i = 1; i < maxLevel; ++i)
            {
                expTree[i - 1] = expCalculator.Calculate(i, maxLevel - 1);
            }
            this.expTree = expTree;
        }
#endif
    }
}
