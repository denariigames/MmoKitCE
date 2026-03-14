using LiteNetLibManager;

namespace MultiplayerARPG
{
    public class UIItemsContainer : UICharacterItems
    {
        public bool pickUpOnSelect = false;
        public bool doNotAskForAmount = true;
        private bool readyToPickUp;

        public ItemsContainerEntity TargetEntity { get; private set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (TargetEntity != null)
                TargetEntity.Items.onOperation += OnItemsOperation;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (TargetEntity != null)
                TargetEntity.Items.onOperation -= OnItemsOperation;
        }

        protected override void Update()
        {
            base.Update();
            if (TargetEntity == null || !GameInstance.PlayingCharacterEntity.IsGameEntityInDistance(TargetEntity))
                Hide();
        }

        public void Show(ItemsContainerEntity targetEntity)
        {
            UpdateData(targetEntity);
            Show();
        }

        protected virtual void OnItemsOperation(LiteNetLibSyncListOp operation, int index, CharacterItem oldItem, CharacterItem newItem)
        {
            UpdateData();
        }

        protected override void OnSelect(UICharacterItem ui)
        {
            base.OnSelect(ui);
            if (pickUpOnSelect && readyToPickUp)
                OnClickPickUpSelectedItem();
        }

        public void OnClickPickUpAllItems()
        {
            GameInstance.PlayingCharacterEntity.CallCmdPickupAllItemsFromContainer(TargetEntity.ObjectId);
        }

        public void OnClickPickUpSelectedItem()
        {
            int selectedIndex = CacheSelectionManager.SelectedUI != null ? CacheSelectionManager.SelectedUI.IndexOfData : -1;
            if (selectedIndex < 0)
                return;
            CharacterItem characterItem = TargetEntity.Items[selectedIndex];
            if (characterItem.amount == 1)
            {
                OnClickPickUpItemsContainerConfirmed(selectedIndex, 1);
            }
            else
            {
                if (doNotAskForAmount)
                {
                    OnClickPickUpItemsContainerConfirmed(selectedIndex, -1);
                }
                else
                {
                    UISceneGlobal.Singleton.ShowInputDialog(LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER.ToString()), LanguageManager.GetText(UITextKeys.UI_MOVE_ITEM_FROM_ITEMS_CONTAINER_DESCRIPTION.ToString()), (amount) =>
                    {
                        OnClickPickUpItemsContainerConfirmed(selectedIndex, amount);
                    }, 1, characterItem.amount, characterItem.amount);
                }
            }
        }

        private void OnClickPickUpItemsContainerConfirmed(int selectedIndex, int amount)
        {
            GameInstance.PlayingCharacterEntity.CallCmdPickupItemFromContainer(TargetEntity.ObjectId, selectedIndex, amount);
        }

        public void UpdateData(ItemsContainerEntity targetEntity)
        {
            if (targetEntity == null || !GameInstance.PlayingCharacterEntity.IsGameEntityInDistance(targetEntity))
            {
                TargetEntity = null;
                return;
            }
            TargetEntity = targetEntity;
            UpdateData();
        }

        protected virtual void UpdateData()
        {
            readyToPickUp = false;
            inventoryType = InventoryType.ItemsContainer;
            UpdateData(GameInstance.PlayingCharacter, TargetEntity ? TargetEntity.Items : null);
            readyToPickUp = true;
        }
    }
}
