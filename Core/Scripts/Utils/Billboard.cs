using UnityEngine;

namespace UtilsComponents
{
    public class Billboard : MonoBehaviour
    {
        public Camera targetCamera;

        public Transform CacheTransform { get; private set; }

        private void OnEnable()
        {
            CacheTransform = transform;
            SetupCamera();
        }

        private bool SetupCamera()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
            return targetCamera != null;
        }

        private void LateUpdate()
        {
            if (!SetupCamera())
                return;
            CacheTransform.rotation = Quaternion.Euler(Quaternion.LookRotation(targetCamera.transform.forward, targetCamera.transform.up).eulerAngles);
        }
    }
}
