using Insthync.CameraAndInput;
using UnityEngine;

namespace UtilsComponents
{
    public class FreeCameraController : MonoBehaviour
    {
        public float movementSpeed = 50f;
        public float lookSensitivity = 50f;

        private void Start()
        {
            InputManager.UseMobileInputOnNonMobile = true;
        }

        void Update()
        {
            var sensitivity = Time.deltaTime * lookSensitivity;
            var newRotationX = transform.localEulerAngles.y + InputManager.GetAxis("Mouse X", false) * sensitivity;
            var newRotationY = transform.localEulerAngles.x - InputManager.GetAxis("Mouse Y", false) * sensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);

            var speed = Time.deltaTime * movementSpeed;
            transform.position += transform.forward * speed * InputManager.GetAxis("Vertical", false);
            transform.position += transform.right * speed * InputManager.GetAxis("Horizontal", false);
        }
    }
}
