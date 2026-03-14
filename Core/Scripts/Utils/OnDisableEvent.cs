using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    [DefaultExecutionOrder(1001)]
    public class OnDisableEvent : MonoBehaviour
    {
        public UnityEvent onDisable = new UnityEvent();
        public float delay = 0f;

        protected virtual void OnDisable()
        {
            if (delay <= 0f)
                Trigger();
            else
                StartCoroutine(DelayTrigger(delay));
        }

        IEnumerator DelayTrigger(float delay)
        {
            yield return null;
            yield return new WaitForSeconds(delay);
            Trigger();
        }

        public void Trigger()
        {
            onDisable.Invoke();
        }
    }
}
