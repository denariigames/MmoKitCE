using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.ARMOR_ITEM_FILE, menuName = GameDataMenuConsts.ARMOR_ITEM_MENU, order = GameDataMenuConsts.ARMOR_ITEM_ORDER)]
    public partial class ArmorItem : BaseDefendEquipmentItem, IArmorItem
    {
        public override string TypeTitle
        {
            get { return ArmorType.Title; }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Armor; }
        }

        [Category("Equipment Settings")]
        [Header("Armor Settings")]
        [SerializeField]
        private ArmorType armorType = null;
        public ArmorType ArmorType
        {
            get { return armorType; }
            set { armorType = value; }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddArmorTypes(ArmorType);
        }
    }
}
