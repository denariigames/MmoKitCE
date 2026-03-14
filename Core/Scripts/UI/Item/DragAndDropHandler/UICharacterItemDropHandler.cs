using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UICharacterItemDropHandler : MonoBehaviour, IDropHandler
    {
        public UICharacterItem uiCharacterItem;
        public bool doNotUnEquipItem;
        public bool doNotSwapOrMergeItem;
        public bool doNotEquipItem;
        public bool doNotMoveToStorage;
        public bool doNotMoveFromStorage;
        public bool doNotSwapOrMergeStorageItem;
        [Tooltip("If this is `TRUE`, it will not swap or merge item which dragging from inventory to storage")]
        public bool doNotSwapOrMergeWithStorageItem;
        [Tooltip("If this is `TRUE`, it will not swap or merge item which dragging from storage to inventory")]
        [FormerlySerializedAs("doNotSwapOrMergeWithNonEquipItem")]
        public bool doNotSwapOrMergeWithInventoryItem;

        protected RectTransform _dropRect;
        public RectTransform DropRect
        {
            get
            {
                if (_dropRect == null)
                    _dropRect = transform as RectTransform;
                return _dropRect;
            }
        }

        protected virtual void Start()
        {
            if (uiCharacterItem == null)
                uiCharacterItem = GetComponent<UICharacterItem>();
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (uiCharacterItem == null)
            {
                Debug.LogWarning("[UICharacterItemDropHandler] `uicharacterItem` is empty");
                return;
            }
            // Validate drop position
            if (!RectTransformUtility.RectangleContainsScreenPoint(DropRect, eventData.position))
                return;
            // Validate dragging UI
            UIDragHandler dragHandler = UIDragHandler.Get(eventData);
            if (dragHandler == null || !dragHandler.CanDrop)
                return;
            // Get dragged item UI. If dragging item UI is UI for character item, equip the item
            UICharacterItemDragHandler draggedItemUI = dragHandler as UICharacterItemDragHandler;
            if (draggedItemUI != null && draggedItemUI.UIItem != uiCharacterItem)
            {
                switch (draggedItemUI.Location)
                {
                    case UICharacterItemDragHandler.SourceLocation.EquipItems:
                        OnDropEquipItem(draggedItemUI);
                        break;
                    case UICharacterItemDragHandler.SourceLocation.NonEquipItems:
                        OnDropNonEquipItem(draggedItemUI);
                        break;
                    case UICharacterItemDragHandler.SourceLocation.StorageItems:
                        OnDropStorageItem(draggedItemUI);
                        break;
                }
            }
        }

        protected virtual void OnDropEquipItem(UICharacterItemDragHandler draggedItemUI)
        {
            // Set UI drop state
            draggedItemUI.IsDropped = true;
            switch (uiCharacterItem.InventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (doNotUnEquipItem)
                        return;
                    // Unequip item
                    GameInstance.ClientInventoryHandlers.RequestUnEquipItem(
                        draggedItemUI.UIItem.InventoryType,
                        draggedItemUI.UIItem.IndexOfData,
                        draggedItemUI.UIItem.EquipSlotIndex,
                        uiCharacterItem.IndexOfData,
                        ClientInventoryActions.ResponseUnEquipArmor,
                        ClientInventoryActions.ResponseUnEquipWeapon);
                    break;
                case InventoryType.StorageItems:
                    if (doNotMoveToStorage)
                        return;
                    // Drop equip item to storage item
                    UIStorageItems uiStorageItems = GetComponentInParent<UIStorageItems>();
                    if (uiStorageItems == null)
                        uiStorageItems = FindFirstObjectByType<UIStorageItems>();
                    if (uiStorageItems == null)
                    {
                        Debug.LogError("Unable to drop non equip item, no opened storage items UI");
                        return;
                    }
                    StorageType storageType = uiStorageItems.StorageType;
                    string storageOwnerId = uiStorageItems.StorageOwnerId;
                    if (doNotSwapOrMergeWithStorageItem)
                    {
                        draggedItemUI.UIItem.OnClickMoveToStorage(storageType, storageOwnerId, -1);
                    }
                    else
                    {
                        draggedItemUI.UIItem.OnClickMoveToStorage(storageType, storageOwnerId, uiCharacterItem.IndexOfData);
                    }
                    break;
            }
        }

        protected virtual void OnDropNonEquipItem(UICharacterItemDragHandler draggedItemUI)
        {
            // Set UI drop state
            draggedItemUI.IsDropped = true;
            switch (uiCharacterItem.InventoryType)
            {
                case InventoryType.NonEquipItems:
                    if (doNotSwapOrMergeItem)
                        return;
                    // Drop non equip item to non equip item
                    GameInstance.ClientInventoryHandlers.RequestSwapOrMergeItem(new RequestSwapOrMergeItemMessage()
                    {
                        fromIndex = draggedItemUI.UIItem.IndexOfData,
                        toIndex = uiCharacterItem.IndexOfData,
                    }, ClientInventoryActions.ResponseSwapOrMergeItem);
                    break;
                case InventoryType.EquipItems:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    if (doNotEquipItem)
                        return;
                    // Drop non equip item to equip item
                    EquipItem(draggedItemUI);
                    break;
                case InventoryType.StorageItems:
                    if (doNotMoveToStorage)
                        return;
                    // Drop non equip item to storage item
                    UIStorageItems uiStorageItems = GetComponentInParent<UIStorageItems>();
                    if (uiStorageItems == null)
                        uiStorageItems = FindFirstObjectByType<UIStorageItems>();
                    if (uiStorageItems == null)
                    {
                        Debug.LogError("Unable to drop non equip item, no opened storage items UI");
                        return;
                    }
                    StorageType storageType = uiStorageItems.StorageType;
                    string storageOwnerId = uiStorageItems.StorageOwnerId;
                    if (doNotSwapOrMergeWithStorageItem)
                    {
                        draggedItemUI.UIItem.OnClickMoveToStorage(storageType, storageOwnerId, -1);
                    }
                    else
                    {
                        draggedItemUI.UIItem.OnClickMoveToStorage(storageType, storageOwnerId, uiCharacterItem.IndexOfData);
                    }
                    break;
            }
        }

        protected void OnDropStorageItem(UICharacterItemDragHandler draggedItemUI)
        {
            // Set UI drop state
            draggedItemUI.IsDropped = true;
            switch (uiCharacterItem.InventoryType)
            {
                case InventoryType.NonEquipItems:
                case InventoryType.EquipItems:
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    if (doNotMoveFromStorage)
                        return;
                    // Drop storage item to non equip item
                    if (doNotSwapOrMergeWithInventoryItem)
                    {
                        draggedItemUI.UIItem.OnClickMoveFromStorage(uiCharacterItem.InventoryType, uiCharacterItem.EquipSlotIndex, -1);
                    }
                    else
                    {
                        draggedItemUI.UIItem.OnClickMoveFromStorage(uiCharacterItem.InventoryType, uiCharacterItem.EquipSlotIndex, uiCharacterItem.IndexOfData);
                    }
                    break;
                case InventoryType.StorageItems:
                    if (doNotSwapOrMergeStorageItem)
                        return;
                    // Drop storage item to storage item
                    UIStorageItems uiStorageItems = GetComponentInParent<UIStorageItems>();
                    if (uiStorageItems == null)
                        uiStorageItems = FindFirstObjectByType<UIStorageItems>();
                    if (uiStorageItems == null)
                    {
                        Debug.LogError("Unable to drop storage item, no opened storage items UI");
                        return;
                    }
                    StorageType storageType = uiStorageItems.StorageType;
                    string storageOwnerId = uiStorageItems.StorageOwnerId;
                    GameInstance.ClientStorageHandlers.RequestSwapOrMergeStorageItem(new RequestSwapOrMergeStorageItemMessage()
                    {
                        storageType = storageType,
                        storageOwnerId = storageOwnerId,
                        fromIndex = draggedItemUI.UIItem.IndexOfData,
                        toIndex = uiCharacterItem.IndexOfData
                    }, ClientStorageActions.ResponseSwapOrMergeStorageItem);
                    break;
            }
        }

        protected void EquipItem(UICharacterItemDragHandler draggedItemUI)
        {
            // Don't equip the item if drop area is not setup as equip slot UI
            if (!uiCharacterItem.IsSetupAsEquipSlot)
                return;

            // Detect type of equipping slot and validate
            IArmorItem armorItem = draggedItemUI.UIItem.CharacterItem.GetArmorItem();
            IWeaponItem weaponItem = draggedItemUI.UIItem.CharacterItem.GetWeaponItem();
            IShieldItem shieldItem = draggedItemUI.UIItem.CharacterItem.GetShieldItem();
            switch (uiCharacterItem.InventoryType)
            {
                case InventoryType.EquipItems:
                    if (armorItem == null ||
                        !armorItem.GetEquipPosition().Equals(uiCharacterItem.EquipPosition))
                    {
                        // Check if it's correct equip position or not
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CANNOT_EQUIP);
                        return;
                    }
                    break;
                case InventoryType.EquipWeaponRight:
                case InventoryType.EquipWeaponLeft:
                    if (weaponItem == null &&
                        shieldItem == null)
                    {
                        // Check if it's correct equip position or not
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_CANNOT_EQUIP);
                        return;
                    }
                    break;
            }
            // Can equip the item
            // so tell the server that this client want to equip the item
            GameInstance.ClientInventoryHandlers.RequestEquipItem(
                draggedItemUI.UIItem.IndexOfData,
                uiCharacterItem.InventoryType,
                uiCharacterItem.EquipSlotIndex,
                ClientInventoryActions.ResponseEquipArmor,
                ClientInventoryActions.ResponseEquipWeapon);
        }
    }
}
