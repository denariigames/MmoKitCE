using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    public class RaycastButton3D : MonoBehaviour
    {
        public UnityEvent onClick = new UnityEvent();

        private void Start()
        {
            Collider collider = GetComponent<Collider>();
            if (collider)
                collider.isTrigger = true;
            RaycastButtonManager3D.SetupInstance();
        }

        public void InvokeOnClick()
        {
            onClick.Invoke();
        }
    }
}
