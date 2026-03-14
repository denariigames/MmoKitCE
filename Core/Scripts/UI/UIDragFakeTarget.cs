using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragFakeTarget : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public UIDragHandler DragHandler { get; set; }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public async void OnEndDrag(PointerEventData eventData)
    {
        DragHandler.OnEndDrag();
        await UniTask.DelayFrame(1);
        Destroy(gameObject);
    }
}
