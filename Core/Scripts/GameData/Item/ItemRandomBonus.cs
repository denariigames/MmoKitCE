using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class ItemRandomBonus
    {
        [Tooltip("0 = Unlimit")]
        public int maxRandomStatsAmount = 0;
        public RandomCharacterStats randomCharacterStats = new RandomCharacterStats();
        public RandomCharacterStats randomCharacterStatsRate = new RandomCharacterStats();
        public AttributeRandomAmount[] randomAttributeAmounts = new AttributeRandomAmount[0];
        public AttributeRandomAmount[] randomAttributeAmountRates = new AttributeRandomAmount[0];
        public ResistanceRandomAmount[] randomResistanceAmounts = new ResistanceRandomAmount[0];
        public ArmorRandomAmount[] randomArmorAmounts = new ArmorRandomAmount[0];
        public ArmorRandomAmount[] randomArmorAmountRates = new ArmorRandomAmount[0];
        public DamageRandomAmount[] randomDamageAmounts = new DamageRandomAmount[0];
        public DamageRandomAmount[] randomDamageAmountRates = new DamageRandomAmount[0];
        public SkillRandomLevel[] randomSkillLevels = new SkillRandomLevel[0];

        public void PrepareRelatesData()
        {
            GameInstance.AddAttributes(randomAttributeAmounts);
            GameInstance.AddAttributes(randomAttributeAmountRates);
            GameInstance.AddDamageElements(randomResistanceAmounts);
            GameInstance.AddDamageElements(randomArmorAmounts);
            GameInstance.AddDamageElements(randomArmorAmountRates);
            GameInstance.AddDamageElements(randomDamageAmounts);
            GameInstance.AddDamageElements(randomDamageAmountRates);
            GameInstance.AddSkills(randomSkillLevels);
        }
    }
}
