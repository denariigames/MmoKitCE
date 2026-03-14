using UnityEngine;
using UnityEngine.EventSystems;

namespace MultiplayerARPG
{
    public partial class UISellItemDropHandler : MonoBehaviour, IDropHandler
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
            // If dragged item UI
            UICharacterItemDragHandler draggedItemUI = dragHandler as UICharacterItemDragHandler;
            if (draggedItemUI != null)
            {
                switch (draggedItemUI.Location)
                {
                    case UICharacterItemDragHandler.SourceLocation.NonEquipItems:
                    case UICharacterItemDragHandler.SourceLocation.EquipItems:
                        draggedItemUI.UIItem.OnClickSell();
                        break;
                }
            }
        }
    }
}
