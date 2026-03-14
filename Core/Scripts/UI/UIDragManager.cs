using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class UIDragManager : MonoBehaviour
{
    private static UIDragManager s_singleton;
    public static UIDragManager Singleton
    {
        get
        {
            if (s_singleton == null)
                s_singleton = new GameObject("__UIDragManager").AddComponent<UIDragManager>();
            return s_singleton;
        }
    }

    public Vector2 IconSize = new Vector2(40, 40);
    private GameObject _ui;

    private void Awake()
    {
        s_singleton = this;
    }

    public async void OnBeginDrag(UIDragHandler dragHandler, Transform container, PointerEventData eventData)
    {
        if (dragHandler == null)
            return;

        _ui = new GameObject("__DraggingIcon");
        _ui.transform.SetParent(container);
        _ui.transform.localScale = Vector3.one;
        _ui.transform.SetPositionAndRotation(eventData.position, Quaternion.identity);

        Image img = _ui.AddComponent<Image>();
        img.preserveAspect = true;
        img.raycastTarget = false;
        img.rectTransform.sizeDelta = IconSize;

        UIDragFakeTarget fakeTarget = _ui.AddComponent<UIDragFakeTarget>();
        fakeTarget.DragHandler = dragHandler;
        eventData.pointerDrag = _ui;

        img.sprite = await dragHandler.LoadIcon();
    }
}