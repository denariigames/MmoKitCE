using Insthync.UnityEditorUtils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct NpcDialogConfirmRequirement
    {
        public int gold;
        [ArrayElementTitle("currency")]
        public CurrencyAmount[] currencyAmounts;
        [ArrayElementTitle("item")]
        public ItemAmount[] itemAmounts;

        public bool HasConfirmConditions()
        {
            return gold > 0 || (currencyAmounts != null && currencyAmounts.Length > 0) || (itemAmounts != null && itemAmounts.Length > 0);
        }
    }
}
