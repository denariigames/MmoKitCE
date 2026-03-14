using UnityEngine;

namespace UtilsComponents
{
    public class CopyCamera : MonoBehaviour
    {
        public Camera copyFromCamera;
        public Camera copyToCamera;
        public bool copyOrthographic = true;
        public bool copyOrthographicSize = true;
        public bool copyNearClipPlane = true;
        public bool copyFarClipPlane = true;
        public bool copyFieldOfView = true;
        public bool copyRect = true;
        public bool copyUsePhysicalProperties = true;
        public bool copyFocalLength = true;
        public bool copySensorSize = true;
        public bool copyLensShift = true;

        void Update()
        {
            if (copyFromCamera == null || copyToCamera == null)
                return;
            if (copyOrthographic)
                copyToCamera.orthographic = copyFromCamera.orthographic;
            if (copyOrthographicSize)
                copyToCamera.orthographicSize = copyFromCamera.orthographicSize;
            if (copyNearClipPlane)
                copyToCamera.nearClipPlane = copyFromCamera.nearClipPlane;
            if (copyFarClipPlane)
                copyToCamera.farClipPlane = copyFromCamera.farClipPlane;
            if (copyFieldOfView)
                copyToCamera.fieldOfView = copyFromCamera.fieldOfView;
            if (copyRect)
                copyToCamera.rect = copyFromCamera.rect;
            if (copyUsePhysicalProperties)
                copyToCamera.usePhysicalProperties = copyFromCamera.usePhysicalProperties;
            if (copyFocalLength)
                copyToCamera.focalLength = copyFromCamera.focalLength;
            if (copySensorSize)
                copyToCamera.sensorSize = copyFromCamera.sensorSize;
            if (copyLensShift)
                copyToCamera.lensShift = copyFromCamera.lensShift;
        }
    }
}
