using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onLeftClick;
    public UnityEvent onMiddleClick;
    public UnityEvent onRightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            onLeftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            onMiddleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            onRightClick.Invoke();
    }
}
