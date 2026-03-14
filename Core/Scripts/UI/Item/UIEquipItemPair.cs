using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UIEquipItemPair
    {
        public ArmorType armorType;
        [Tooltip("Example: If this is Ring 1, set this to 0. If this is Ring 2, set this to 1.")]
        [Range(0, 15)]
        public byte equipSlotIndex;
        public UICharacterItem ui;
    }
}
