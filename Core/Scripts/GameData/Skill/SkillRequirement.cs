using Insthync.UnityEditorUtils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class SkillRequirement
    {
        public bool disallow = false;
        public IncrementalInt characterLevel = new IncrementalInt();
        public IncrementalFloat skillPoint = new IncrementalFloat();
        public IncrementalInt gold = new IncrementalInt();
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
