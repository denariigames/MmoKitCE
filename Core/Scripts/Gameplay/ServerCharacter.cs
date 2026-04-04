using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ServerCharacter : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 10f;
        public float fastSpeed = 25f;

        [Header("Mouse Look")]
        public float mouseSensitivity = 2.5f;
        public float minY = -90f;
        public float maxY = 90f;

        private float yaw;
        private float pitch;

        private void Start()
        {
            Vector3 angles = transform.eulerAngles;
            yaw = angles.y;
            pitch = angles.x;
        }

        private void Update()
        {
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            if (InputManager.GetButton("CameraRotate"))
            {
                float mouseX = InputManager.GetAxis("Mouse X", false) * mouseSensitivity * 100f * Time.deltaTime;
                float mouseY = InputManager.GetAxis("Mouse Y", false) * mouseSensitivity * 100f * Time.deltaTime;

                yaw += mouseX;
                pitch -= mouseY;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            pitch = Mathf.Clamp(pitch, minY, maxY);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        private void HandleMovement()
        {
            float speed = InputManager.GetButton("Sprint") ? fastSpeed : moveSpeed;

            Vector3 move = new Vector3(
                InputManager.GetAxis("Horizontal", false),
                0f,
                InputManager.GetAxis("Vertical", false)
            );

            Vector3 velocity = transform.TransformDirection(move) * speed;
            transform.position += velocity * Time.deltaTime;
        }
    }
}
