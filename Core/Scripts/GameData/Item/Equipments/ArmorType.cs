using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ARMOR_TYPE_FILE, menuName = GameDataMenuConsts.ARMOR_TYPE_MENU, order = GameDataMenuConsts.ARMOR_TYPE_ORDER)]
    public partial class ArmorType : BaseGameData
    {
        [Category("Armor Type Settings")]
        [SerializeField]
        [Tooltip("Example: If you want to make it can equip 4 rings, set this to 4")]
        [Range(1, 16)]
        private byte equippableSlots = 1;
        public byte EquippableSlots { get { return equippableSlots; } }

        [SerializeField]
        [Tooltip("What kind of position (or slot) which can put this item into it.")]
        private string equipPosition = string.Empty;
        public string EquipPosition
        {
            get { return (string.IsNullOrEmpty(equipPosition) ? Id : equipPosition).ToUpper(); }
        }

        public ArmorType GenerateDefaultArmorType()
        {
            name = GameDataConst.UNKNOW_ARMOR_TYPE_ID;
            defaultTitle = GameDataConst.UNKNOW_ARMOR_TYPE_TITLE;
            return this;
        }
    }
}
