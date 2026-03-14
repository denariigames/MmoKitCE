using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    public class RepeatingEvent : MonoBehaviour
    {
        public float firstDelay;
        public float repeatDelay;
        public UnityEvent repeating = new UnityEvent();

        private void OnEnable()
        {
            InvokeRepeating(nameof(Repeating), firstDelay, repeatDelay);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }

        private void Repeating()
        {
            repeating.Invoke();
        }
    }
}
