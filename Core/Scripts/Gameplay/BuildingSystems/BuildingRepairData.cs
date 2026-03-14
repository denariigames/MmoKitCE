using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct BuildingRepairData
    {
        public bool canRepairFromMenu;
        [Tooltip("This will being used if `canRepairFromMenu` is `FALSE`, set the weapon which can be used to repair the building")]
        public BaseItem weaponItem;
        [Tooltip("Max recovery HP for one time, for example: if building's HP is `80/100`, and this value is `10` then it will recover `10` HP, not `20`, and if building's HP is `80/100`, and this value is `30` then it will recover `20` HP")]
        public int maxRecoveryHp;
        [Tooltip("Require gold to recovery `1` building HP")]
        public int requireGold;
        [Tooltip("Require items to recovery `1` building HP")]
        [ArrayElementTitle("item")]
        public ItemAmount[] requireItems;
        [Tooltip("Require currencies to recovery `1` building HP")]
        [ArrayElementTitle("currency")]
        public CurrencyAmount[] requireCurrencies;
    }
}