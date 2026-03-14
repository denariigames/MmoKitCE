using UnityEngine;
using UnityEngine.EventSystems;

namespace MultiplayerARPG
{
    public partial class UIEquipItemSlotDropHandler : MonoBehaviour, IDropHandler
    {
        public UICharacterItem uiCharacterItem;

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
                Debug.LogWarning("[UIEquipItemSlotDropHandler] `uicharacterItem` is empty");
                return;
            }
            // Validate drop position
            if (!RectTransformUtility.RectangleContainsScreenPoint(DropRect, eventData.position))
                return;
            // Validate dragging UI
            UIDragHandler dragHandler = UIDragHandler.Get(eventData);
            if (dragHandler == null || !dragHandler.CanDrop)
                return;
            // Set UI drop state
            dragHandler.IsDropped = true;
            // Get dragged item UI. If dragging item UI is UI for character item, equip the item
            UICharacterItemDragHandler draggedItemUI = dragHandler as UICharacterItemDragHandler;
            if (draggedItemUI != null)
            {
                switch (draggedItemUI.Location)
                {
                    case UICharacterItemDragHandler.SourceLocation.EquipItems:
                        break;
                    case UICharacterItemDragHandler.SourceLocation.NonEquipItems:
                        // If dropped non equipped equipment item to equip slot, equip it
                        EquipItem(draggedItemUI);
                        break;
                    case UICharacterItemDragHandler.SourceLocation.StorageItems:
                        draggedItemUI.UIItem.OnClickMoveFromStorage(uiCharacterItem.InventoryType, uiCharacterItem.EquipSlotIndex, uiCharacterItem.IndexOfData);
                        break;
                }
            }
        }

        protected virtual void EquipItem(UICharacterItemDragHandler draggedItemUI)
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
