using UnityEngine;
using UnityEngine.EventSystems;

namespace MultiplayerARPG
{
    public partial class NonEquipDropHandler : MonoBehaviour, IDropHandler
    {
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

        public virtual void OnDrop(PointerEventData eventData)
        {
            // Validate drop position
            if (!RectTransformUtility.RectangleContainsScreenPoint(DropRect, eventData.position))
                return;
            // Validate dragging UI
            UIDragHandler dragHandler = UIDragHandler.Get(eventData);
            if (dragHandler == null || !dragHandler.CanDrop)
                return;
            // Set UI drop state
            dragHandler.IsDropped = true;
            // Get dragged item UI. if dragging item UI is UI for character item, unequip the item
            UICharacterItemDragHandler draggedItemUI = dragHandler as UICharacterItemDragHandler;
            if (draggedItemUI != null)
            {
                switch (draggedItemUI.Location)
                {
                    case UICharacterItemDragHandler.SourceLocation.EquipItems:
                        draggedItemUI.UIItem.OnClickUnEquip();
                        break;
                    case UICharacterItemDragHandler.SourceLocation.NonEquipItems:
                        break;
                    case UICharacterItemDragHandler.SourceLocation.StorageItems:
                        draggedItemUI.UIItem.OnClickMoveFromStorage();
                        break;
                    case UICharacterItemDragHandler.SourceLocation.Vending:
                        draggedItemUI.UIItem.OnClickBuyVendingItem();
                        break;
                }
            }
        }
    }
}
