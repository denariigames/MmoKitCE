using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract partial class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public static GameObject DraggingObject = null;
    public enum ScrollRectAllowing
    {
        None,
        AllowVerticalScrolling,
        AllowHorizontalScrolling,
    }

    public ScrollRectAllowing scrollRectAllowing;
    public ScrollRect scrollRect;
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onBeginDrag = new UnityEvent();
    public UnityEvent onEndDrag = new UnityEvent();

    public Canvas CacheCanvas { get; protected set; }
    public virtual bool CanDrag { get { return true; } }
    public virtual bool CanDrop { get { return CanDrag && !IsDropped; } }
    public bool IsDropped { get; set; }

    protected virtual void Start()
    {
        if (scrollRect == null)
            scrollRect = GetComponentInParent<ScrollRect>();

        CacheCanvas = GetComponentInParent<Canvas>();
        // Find root canvas, will use it to set as parent while dragging
        if (CacheCanvas != null)
            CacheCanvas = CacheCanvas.rootCanvas;

        onStart.Invoke();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (DraggingObject != null)
            return;

        if (scrollRect != null)
        {
            if (scrollRectAllowing == ScrollRectAllowing.AllowVerticalScrolling &&
                Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y))
            {
                scrollRect.SendMessage("OnBeginDrag", eventData);
                eventData.pointerDrag = scrollRect.gameObject;
                return;
            }

            if (scrollRectAllowing == ScrollRectAllowing.AllowHorizontalScrolling &&
                Mathf.Abs(eventData.delta.y) < Mathf.Abs(eventData.delta.x))
            {
                scrollRect.SendMessage("OnBeginDrag", eventData);
                eventData.pointerDrag = scrollRect.gameObject;
                return;
            }
        }

        if (!CanDrag)
            return;

        UIDragManager.Singleton.OnBeginDrag(this, CacheCanvas.transform, eventData);
        DraggingObject = gameObject;
        IsDropped = false;
        onBeginDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Do nothing, it is required to make on begin drag work
    }

    public virtual void OnEndDrag()
    {
        // NOTE: It might have issues when someone dragging more than one and end drag later
        DraggingObject = null;
        onEndDrag.Invoke();
    }

    public static UIDragHandler Get(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return null;
        UIDragFakeTarget dragFakeTarget = eventData.pointerDrag.GetComponent<UIDragFakeTarget>();
        if (dragFakeTarget == null)
            return null;
        return dragFakeTarget.DragHandler;
    }

    public abstract UniTask<Sprite> LoadIcon();
}
