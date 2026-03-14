using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.JUNK_ITEM_FILE, menuName = GameDataMenuConsts.JUNK_ITEM_MENU, order = GameDataMenuConsts.JUNK_ITEM_ORDER)]
    public partial class JunkItem : BaseItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_JUNK.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Junk; }
        }
    }
}
