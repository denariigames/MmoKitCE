using System.Collections.Generic;

namespace MultiplayerARPG
{
	public class UIPickupItemList : UICharacterItems
    {
        public bool pickUpOnSelect;
        private bool readyToPickUp;

        protected override void OnSelect(UICharacterItem ui)
        {
            base.OnSelect(ui);
            if (pickUpOnSelect && readyToPickUp)
                OnClickPickUpSelectedItem();
        }

        public void OnClickPickUpSelectedItem()
        {
            string selectedId = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.CharacterItem.id : string.Empty;
            if (string.IsNullOrEmpty(selectedId))
                return;
            GameInstance.PlayingCharacterEntity.CallCmdPickup(uint.Parse(selectedId));
        }

        public void OnClickPickupNearbyItems()
        {
            GameInstance.PlayingCharacterEntity.CallCmdPickupNearbyItems();
        }

        public void UpdateData(IList<CharacterItem> characterItems)
        {
            readyToPickUp = false;
            inventoryType = InventoryType.Unknow;
            UpdateData(GameInstance.PlayingCharacter, characterItems);
            readyToPickUp = true;
        }
    }
}
