using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIStorageItems : UICharacterItems
    {
        [Header("Storage String Formats")]
        [Tooltip("Format => {0} = {Current Total Weights}, {1} = {Weight Limit}")]
        public UILocaleKeySetting formatKeyWeightLimit = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_WEIGHT);
        [Tooltip("Format => {0} = {Current Used Slots}, {1} = {Slot Limit}")]
        public UILocaleKeySetting formatKeySlotLimit = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CURRENT_SLOT);

        [Header("Storage UI Elements")]
        public TextWrapper uiTextWeightLimit;
        public TextWrapper uiTextSlotLimit;

        public StorageType StorageType { get; private set; }
        public string StorageOwnerId { get; private set; }
        public BaseGameEntity TargetEntity { get; private set; }
        public int WeightLimit { get; private set; }
        public int SlotLimit { get; private set; }
        public float TotalWeight { get; private set; }
        public int UsedSlots { get; private set; }

        private bool _doNotCloseStorageOnDisable = true;

        protected override void OnEnable()
        {
            inventoryType = InventoryType.StorageItems;
            StorageId storageId = new StorageId(StorageType, StorageOwnerId);
            if (GameInstance.OpenedStorages.TryGetValue(storageId, out List<CharacterItem> updatedItems))
                UpdateData(StorageType, StorageOwnerId, updatedItems);
            ClientStorageActions.onNotifyStorageItemsUpdated += UpdateData;
            onGenerateEntry += OnGenerateEntry;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            ClientStorageActions.onNotifyStorageItemsUpdated -= UpdateData;
            onGenerateEntry -= OnGenerateEntry;
            // Close storage
            if (!_doNotCloseStorageOnDisable)
                GameInstance.ClientStorageHandlers.RequestCloseStorage(ClientStorageActions.ResponseCloseStorage);
            _doNotCloseStorageOnDisable = true;
            // Clear data
            StorageType = StorageType.None;
            StorageOwnerId = string.Empty;
            TargetEntity = null;
            WeightLimit = 0;
            SlotLimit = 0;
            base.OnDisable();
        }

        public override void Hide()
        {
            _doNotCloseStorageOnDisable = false;
            base.Hide();
        }

        public void Hide(bool doNotCloseStorageOnDisable)
        {
            _doNotCloseStorageOnDisable = doNotCloseStorageOnDisable;
            base.Hide();
        }

        protected override void Update()
        {
            if (uiTextWeightLimit != null)
            {
                if (WeightLimit <= 0)
                    uiTextWeightLimit.text = LanguageManager.GetText(UITextKeys.UI_LABEL_UNLIMIT_WEIGHT.ToString());
                else
                    uiTextWeightLimit.text = ZString.Format(LanguageManager.GetText(formatKeyWeightLimit), TotalWeight.ToString("N2"), WeightLimit.ToString("N2"));
            }

            if (uiTextSlotLimit != null)
            {
                if (SlotLimit <= 0)
                    uiTextSlotLimit.text = LanguageManager.GetText(UITextKeys.UI_LABEL_UNLIMIT_SLOT.ToString());
                else
                    uiTextSlotLimit.text = ZString.Format(LanguageManager.GetText(formatKeySlotLimit), UsedSlots.ToString("N0"), SlotLimit.ToString("N0"));
            }

            base.Update();
        }

        public void Show(StorageType storageType, string storageOwnerId, BaseGameEntity targetEntity, int weightLimit, int slotLimit)
        {
            StorageType = storageType;
            StorageOwnerId = storageOwnerId;
            TargetEntity = targetEntity;
            WeightLimit = weightLimit;
            SlotLimit = slotLimit;
            Show();
        }

        public virtual void UpdateData(StorageType storageType, string storageOwnerId, IList<CharacterItem> characterItems)
        {
            if (storageType != StorageType || !string.Equals(storageOwnerId, StorageOwnerId))
                return;
            UpdateData(GameInstance.PlayingCharacter, characterItems);
        }

        public override void GenerateList()
        {
            // Reset total weight and used slots, it will be increased in `OnGenerateEntry` function
            TotalWeight = 0;
            UsedSlots = 0;
            base.GenerateList();
        }

        private void OnGenerateEntry(int indexOfData, CharacterItem characterItem, int indexOfUi, UICharacterItem ui)
        {
            if (!characterItem.IsEmptySlot())
            {
                // Increase total weight and used slots, if data is not empty slot
                TotalWeight += characterItem.GetItem().Weight * characterItem.amount;
                if (!characterItem.GetItem().NoSlotUsage)
                    UsedSlots++;
            }
            ui.StorageType = StorageType;
            ui.StorageOwnerId = StorageOwnerId;
        }
    }
}
