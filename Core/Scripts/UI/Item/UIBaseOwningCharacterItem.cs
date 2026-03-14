using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class UIBaseOwningCharacterItem : UISelectionEntry<UIOwningCharacterItemData>
    {
        public InventoryType InventoryType { get { return Data.inventoryType; } }
        public int IndexOfData { get { return Data.indexOfData; } }
        public byte EquipSlotIndex { get { return Data.equipSlotIndex; } }
        public CharacterItem CharacterItem
        {
            get
            {
                switch (InventoryType)
                {
                    case InventoryType.NonEquipItems:
                        if (IndexOfData >= 0 && IndexOfData < GameInstance.PlayingCharacter.NonEquipItems.Count)
                            return GameInstance.PlayingCharacter.NonEquipItems[IndexOfData];
                        break;
                    case InventoryType.EquipItems:
                        if (IndexOfData >= 0 && IndexOfData < GameInstance.PlayingCharacter.EquipItems.Count)
                            return GameInstance.PlayingCharacter.EquipItems[IndexOfData];
                        break;
                    case InventoryType.EquipWeaponRight:
                        return GameInstance.PlayingCharacter.SelectableWeaponSets[EquipSlotIndex].rightHand;
                    case InventoryType.EquipWeaponLeft:
                        return GameInstance.PlayingCharacter.SelectableWeaponSets[EquipSlotIndex].leftHand;
                }
                return CharacterItem.Empty;
            }
        }
        public int Level { get { return CharacterItem.level; } }
        public int Amount { get { return CharacterItem.amount; } }

        public UICharacterItem uiCharacterItem;
        [Tooltip("These objects will be activated while item is set")]
        public GameObject[] hasItemObjects;
        [Tooltip("These objects will be activated while item is not set")]
        public GameObject[] noItemObjects;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCharacterItem = null;
            hasItemObjects.Nullify();
            noItemObjects.Nullify();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onEquipItemsOperation += PlayingCharacterEntity_onEquipItemsOperation;
            GameInstance.PlayingCharacterEntity.onNonEquipItemsOperation += PlayingCharacterEntity_onNonEquipItemsOperation;
            GameInstance.PlayingCharacterEntity.onSelectableWeaponSetsOperation += PlayingCharacterEntity_onSelectableWeaponSetsOperation;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onEquipItemsOperation -= PlayingCharacterEntity_onEquipItemsOperation;
            GameInstance.PlayingCharacterEntity.onNonEquipItemsOperation -= PlayingCharacterEntity_onNonEquipItemsOperation;
            GameInstance.PlayingCharacterEntity.onSelectableWeaponSetsOperation -= PlayingCharacterEntity_onSelectableWeaponSetsOperation;
        }

        private void PlayingCharacterEntity_onEquipItemsOperation(LiteNetLibSyncListOp op, int itemIndex, CharacterItem oldItem, CharacterItem newItem)
        {
            if (InventoryType != InventoryType.EquipItems || IndexOfData != itemIndex)
                return;
            OnUpdateCharacterItems();
        }

        private void PlayingCharacterEntity_onNonEquipItemsOperation(LiteNetLibSyncListOp op, int itemIndex, CharacterItem oldItem, CharacterItem newItem)
        {
            if (InventoryType != InventoryType.NonEquipItems || IndexOfData != itemIndex)
                return;
            OnUpdateCharacterItems();
        }

        private void PlayingCharacterEntity_onSelectableWeaponSetsOperation(LiteNetLibSyncListOp op, int itemIndex, EquipWeapons oldItem, EquipWeapons newItem)
        {
            if ((InventoryType != InventoryType.EquipWeaponLeft && InventoryType != InventoryType.EquipWeaponRight) || EquipSlotIndex != itemIndex)
                return;
            OnUpdateCharacterItems();
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();
            if (hasItemObjects != null && hasItemObjects.Length > 0)
            {
                foreach (GameObject hasItemObject in hasItemObjects)
                {
                    if (hasItemObject == null)
                        continue;
                    hasItemObject.SetActive(!CharacterItem.IsEmptySlot());
                }
            }
            if (noItemObjects != null && noItemObjects.Length > 0)
            {
                foreach (GameObject noItemObject in noItemObjects)
                {
                    if (noItemObject == null)
                        continue;
                    noItemObject.SetActive(CharacterItem.IsEmptySlot());
                }
            }
        }

        protected override void UpdateData()
        {
            if (uiCharacterItem != null)
            {
                if (CharacterItem.IsEmptySlot())
                {
                    uiCharacterItem.Hide();
                }
                else
                {
                    uiCharacterItem.Setup(new UICharacterItemData(CharacterItem, InventoryType), GameInstance.PlayingCharacter, IndexOfData);
                    uiCharacterItem.Show();
                }
            }
        }

        public abstract void OnUpdateCharacterItems();
    }
}
