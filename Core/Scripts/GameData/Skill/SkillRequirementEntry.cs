using Insthync.UnityEditorUtils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class SkillRequirementEntry
    {
        public bool disallow;
        public int characterLevel;
        public float skillPoint;
        public int gold;
        [ArrayElementTitle("attribute")]
        public AttributeAmount[] attributeAmounts = new AttributeAmount[0];
        [ArrayElementTitle("skill")]
        public SkillLevel[] skillLevels = new SkillLevel[0];
        [ArrayElementTitle("currency")]
        public CurrencyAmount[] currencyAmounts = new CurrencyAmount[0];
        [ArrayElementTitle("item")]
        public ItemAmount[] itemAmounts = new ItemAmount[0];
    }
}
