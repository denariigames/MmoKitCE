namespace MultiplayerARPG
{
    public class UIWeaponSet : UIBase
    {
        public UICharacterItem uiRightHandItem;
        public UICharacterItem uiLeftHandItem;

        public UIWeaponSets UIWeaponSets { get; set; }
        public byte Set { get; private set; }
        public EquipWeapons EquipWeapons { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiRightHandItem = null;
            uiLeftHandItem = null;
            UIWeaponSets = null;
        }

        public void SetData(UIWeaponSets uiWeaponSets, byte set, EquipWeapons equipWeapons)
        {
            UIWeaponSets = uiWeaponSets;
            Set = set;
            EquipWeapons = equipWeapons;
            if (uiRightHandItem != null)
                uiRightHandItem.Data = new UICharacterItemData(equipWeapons.rightHand, InventoryType.EquipWeaponRight);
            if (uiLeftHandItem != null)
                uiLeftHandItem.Data = new UICharacterItemData(equipWeapons.leftHand, InventoryType.EquipWeaponLeft);
        }

        public void OnClickChangeWeaponSet()
        {
            UIWeaponSets.ChangeWeaponSet(Set);
            UICharacterHotkeys.SetUsingHotkey(null);
        }
    }
}
