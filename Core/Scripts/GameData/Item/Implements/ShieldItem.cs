using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SHIELD_ITEM_FILE, menuName = GameDataMenuConsts.SHIELD_ITEM_MENU, order = GameDataMenuConsts.SHIELD_ITEM_ORDER)]
    public partial class ShieldItem : BaseDefendEquipmentItem, IShieldItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_SHIELD.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Shield; }
        }

        [Category("In-Scene Objects/Appearance")]
        [SerializeField]
        private EquipmentModel[] sheathModels = new EquipmentModel[0];
        public EquipmentModel[] SheathModels
        {
            get { return sheathModels; }
            set { sheathModels = value; }
        }
    }
}
