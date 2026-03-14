using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    [DefaultExecutionOrder(1000)]
    public class OnEnableEvent : MonoBehaviour
    {
        public static List<OnEnableEvent> checkPoints = new List<OnEnableEvent>();

        public UnityEvent onEnable = new UnityEvent();
        public float delay = 0f;
        public bool isCheckPoint;

        protected virtual void OnEnable()
        {
            if (delay <= 0f)
                Trigger();
            else
                StartCoroutine(DelayTrigger(delay));
            if (isCheckPoint)
            {
                checkPoints.Remove(this);
                checkPoints.Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (isCheckPoint)
            {
                checkPoints.Remove(this);
            }
        }

        IEnumerator DelayTrigger(float delay)
        {
            yield return null;
            yield return new WaitForSeconds(delay);
            Trigger();
        }

        public void Trigger()
        {
            onEnable.Invoke();
        }

        public void TriggerLastCheckPoint()
        {
            if (checkPoints.Count <= 0)
                return;
            checkPoints[checkPoints.Count - 1].Trigger();
        }
    }
}
