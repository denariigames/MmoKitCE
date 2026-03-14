using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    public class OnToggleEvent : MonoBehaviour
    {
        public UnityEvent onToggleOn;
        public UnityEvent onToggleOff;

        public void OnToggle(bool isOn)
        {
            if (isOn)
                onToggleOn.Invoke();
            else
                onToggleOff.Invoke();
        }
    }
}