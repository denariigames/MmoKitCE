using UnityEngine;

namespace UtilsComponents
{
    public class DistanceToCameraBasedScaling : MonoBehaviour
    {
        public float oneScaledDistance = 5f;
        public float scale = 1f;
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
            CacheTransform.localScale = Vector3.one * (Vector3.Distance(targetCamera.transform.position, CacheTransform.position) / oneScaledDistance) * scale;
        }
    }
}
