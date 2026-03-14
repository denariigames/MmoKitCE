using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public static class SelectableNavigationUtils
    {
        public static void ScrollSnap(this Selectable selectable)
        {
            if (selectable == null)
                return;
            selectable.ScrollSnap(selectable.GetComponentInParent<ScrollRect>());
        }

        public static void ScrollSnap(this Selectable selectable, ScrollRect scrollRect)
        {
            if (selectable == null)
                return;
            if (scrollRect == null)
                return;
            RectTransform mask = scrollRect.content.parent as RectTransform;
            Rect maskRect = mask.GetWorldRect();
            Rect thisRect = (selectable.transform as RectTransform).GetWorldRect();
            if (maskRect.ContainsRect(thisRect))
                return;
            float scrollingX = 0f;
            float scrollingY = 0f;
            if (scrollRect.horizontal)
            {
                if (maskRect.center.x > thisRect.center.x)
                {
                    // This rect is left mask rect
                    scrollingX = maskRect.xMin - thisRect.xMin;
                }
                else
                {
                    // This rect is right mask rect
                    scrollingX = maskRect.xMax - thisRect.xMax;
                }
            }
            if (scrollRect.vertical)
            {
                if (maskRect.center.y > thisRect.center.y)
                {
                    // This rect is below mask rect
                    scrollingY = maskRect.yMin - thisRect.yMin;
                }
                else
                {
                    // This rect is above mask rect
                    scrollingY = maskRect.yMax - thisRect.yMax;
                }
            }
            Vector2 newPosition = scrollRect.content.anchoredPosition + (Vector2.right * scrollingX) + (Vector2.up * scrollingY);
            scrollRect.content.anchoredPosition = newPosition;
        }
    }
}
